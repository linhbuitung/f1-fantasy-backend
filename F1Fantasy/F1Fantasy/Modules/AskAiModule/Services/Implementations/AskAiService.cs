using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AskAiModule.Dtos.Api;
using F1Fantasy.Modules.AskAiModule.Dtos.Create;
using F1Fantasy.Modules.AskAiModule.Dtos.Get;
using F1Fantasy.Modules.AskAiModule.Dtos.Mapper;
using F1Fantasy.Modules.AskAiModule.Extensions;
using F1Fantasy.Modules.AskAiModule.Repositories.Interfaces;
using F1Fantasy.Modules.AskAiModule.Services.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace F1Fantasy.Modules.AskAiModule.Services.Implementations;

public class AskAiService(
    UserManager<ApplicationUser> userManager, 
    IAskAiRepository askAiRepository, 
    AskAiClient askAiClient, 
    WooF1Context context, 
    IStaticDataRepository staticDataRepository,
    IConfiguration configuration) : IAskAIService
{
    public async Task AddAskAiCreditAsync(int userId)
    {
        await AddAskAiCreditAsync(userId.ToString());
    }

    // string userId will be converted to int 
    public async Task AddAskAiCreditAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.AskAiCredits += 1;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to add AI credit");
        }
    }
    
    public async Task<List<Dtos.Get.PredictionGetDto>> GetPagedPredictionsByUserIdAsync(int userId, int pageNumber, int pageSize)
    {
        var allPredictions = await askAiRepository.GetAllPredictionsByUserIdAsync(userId);
        var pagedPredictions = allPredictions
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

 
        return pagedPredictions.Select(AskAiDtoMapper.MapPredictionToGetDtoMinimal).ToList();
    }

    public async Task<Dtos.Get.PredictionGetDto> GetPredictionDetailByIdAsync(int predictionId)
    {
        var prediction = await askAiRepository.GetPredictionDetailByIdAsync(predictionId);
        if (prediction == null)
        {
            throw new NotFoundException("Prediction not found");
        }
        return AskAiDtoMapper.MapPredictionToGetDtoDetail(prediction);
    }
    
     public async Task<int> MakeMainRacePredictionAsNewPredictionAsync(int userId,
        Dtos.Create.MainRacePredictionCreateAsNewDto mainRacePredictionCreateAsNewDto)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        
        if (user.AskAiCredits <= 0)
        {
            throw new InvalidOperationException("Not enough AI credits");
        }

        var (driverIdToCode, 
            driverCodeToId, 
            constructorIdToCode, 
            constructorCodeToId, 
            circuitCode) = await ValidateInputExistenceForLocalDatabase(mainRacePredictionCreateAsNewDto);      
        ValidateUniqueDriverConstructorPair(mainRacePredictionCreateAsNewDto.Entries);
        ValidatePredictionCountCap(mainRacePredictionCreateAsNewDto.Entries.Count);
        
        var apiInputDto = AskAiDtoMapper.MapMainRaceCreateAsNewDtoToApiInputDto(mainRacePredictionCreateAsNewDto, driverIdToCode, constructorIdToCode, circuitCode);
        var apiStatusInputDto = AskAiDtoMapper.MapMainRaceCreateAsNewDtoToStatusApiInputDto(mainRacePredictionCreateAsNewDto, driverIdToCode, constructorIdToCode, circuitCode);

        await ValidateInputExistenceForMlModel(apiInputDto);
        
        var predictionResults = await askAiClient.CallMainRacePrediction(apiInputDto);
        if (predictionResults == null || predictionResults.Predictions.Count != mainRacePredictionCreateAsNewDto.Entries.Count)
        {
            throw new Exception("Invalid main race prediction response from AI service");
        }
        
        var predictionStatusResults = await askAiClient.CallStatusPrediction(apiStatusInputDto);
        if (predictionStatusResults == null || predictionStatusResults.Percentages.Count != mainRacePredictionCreateAsNewDto.Entries.Count)
        {
            throw new Exception("Invalid status prediction response from AI service");
        }
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        var processedMainRacePredictionGetDtos = ProcessMainRacePredictionGetDtos(predictionResults, 
            driverCodeToId, 
            constructorCodeToId);
        
        var processedStatusDtos = ProcessStatusPredictionGetDtos(predictionStatusResults, 
            driverCodeToId, 
            constructorCodeToId);
        try
        {
            var newPrediction = AskAiDtoMapper.MapMainRaceCreateDtoToPrediction(userId, mainRacePredictionCreateAsNewDto);
            var createdPrediction = await askAiRepository.AddPredictionAsync(newPrediction);
            
            var driverPredictions = AskAiDtoMapper.MapApiResultToNewDriverPrediction(createdPrediction.Id, 
                processedMainRacePredictionGetDtos, 
                processedStatusDtos);
            // Move All Crashed to the bottom and reorder final positions accordingly
            var orderedDriverPredictions = driverPredictions
                .OrderBy(dp => dp.Crashed) // False (not crashed) comes before True (crashed)
                .ThenBy(dp => dp.FinalPosition) // Then order by final position
                .ToList();
            for (int i = 0; i < orderedDriverPredictions.Count; i++)
            {
                orderedDriverPredictions[i].FinalPosition = i + 1;
            }
            await askAiRepository.AddDriverPredictionsAsync(orderedDriverPredictions);
            
            user.AskAiCredits -= 1;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to deduct AI credit");
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return createdPrediction.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating main race prediction as new: {ex.Message}");

            
            throw;
        }
    }

    public async Task<int> MakeQualifyingPredictionAsync(int userId,
        Dtos.Create.QualifyingPredictionCreateDto qualifyingPredictionCreateDto)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        
        if (user.AskAiCredits <= 0)
        {
            throw new InvalidOperationException("Not enough AI credits");
        }

        var (driverIdToCode, 
            driverCodeToId, 
            constructorIdToCode, 
            constructorCodeToId, 
            circuitCode) = await ValidateInputExistenceForLocalDatabase(qualifyingPredictionCreateDto);      
        ValidateUniqueDriverConstructorPair(qualifyingPredictionCreateDto.Entries);
        ValidatePredictionCountCap(qualifyingPredictionCreateDto.Entries.Count);
        
        var apiInputDto = AskAiDtoMapper.MapQualifyingCreateDtoToApiInputDto(qualifyingPredictionCreateDto, driverIdToCode, constructorIdToCode, circuitCode);
        
        await ValidateInputExistenceForMlModel(apiInputDto);
        
        var predictionResults = await askAiClient.CallQualifyingPrediction(apiInputDto);
        if (predictionResults == null || predictionResults.Predictions.Count != qualifyingPredictionCreateDto.Entries.Count)
        {
            throw new Exception("Invalid qualifying prediction response from AI service");
        }
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        var processedMainRacePredictionGetDtos = ProcessQualifyingPredictionGetDtos(predictionResults, 
            driverCodeToId, 
            constructorCodeToId);

        try
        {
            var newPrediction = AskAiDtoMapper.MapQualifyingCreateDtoToPrediction(userId, qualifyingPredictionCreateDto);
            var createdPrediction = await askAiRepository.AddPredictionAsync(newPrediction);
            
            var driverPredictions = AskAiDtoMapper.MapApiResultToNewDriverPrediction(createdPrediction.Id, 
                processedMainRacePredictionGetDtos);
            
            await askAiRepository.AddDriverPredictionsAsync(driverPredictions);
            
            user.AskAiCredits -= 1;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to deduct AI credit");
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return createdPrediction.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating qualifying prediction: {ex.Message}");

            
            throw;
        }
    }

    public async Task<int> MakeMainRacePredictionFromAlreadyMadeQualifyingPredictionAsync(int userId,
        int predictionId, Dtos.Create.MainRacePredictionCreateAsAdditionDto mainRacePredictionCreateAsAdditionDto)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        
        if (user.AskAiCredits <= 0)
        {
            throw new InvalidOperationException("Not enough AI credits");
        }

        var existingPrediction = await askAiRepository.GetPredictionDetailByIdAsync(predictionId);
        if (existingPrediction == null)
        {
            throw new NotFoundException("Existing prediction not found");
        }
        if (existingPrediction.UserId != userId)
        {
            throw new InvalidOperationException("You can only use your own predictions");
        }
        if(existingPrediction.DriverPredictions.Any(dp => dp.FinalPosition != null))
        {
            throw new InvalidOperationException("This prediction already has a main race prediction associated with it");
        }
        
        var (driverIdToCode, 
            driverCodeToId, 
            constructorIdToCode, 
            constructorCodeToId) = await CreateDictionariesExtensionFromPrediction(existingPrediction);

        var apiInputDtos = AskAiDtoMapper.MapMainRaceCreateAsAdditionToApiInputDto(mainRacePredictionCreateAsAdditionDto, existingPrediction);
        var apiStatusInputDto = AskAiDtoMapper.MapMainRaceCreateAsAdditionDtoToStatusApiInputDto( existingPrediction.DriverPredictions.ToList() ,mainRacePredictionCreateAsAdditionDto, driverIdToCode, constructorIdToCode, existingPrediction.Circuit.Code);

        var predictionResults = await askAiClient.CallMainRacePrediction(apiInputDtos);
        if (predictionResults == null || predictionResults.Predictions.Count != existingPrediction.DriverPredictions.Count)
        {
            throw new Exception("Invalid main race prediction response from AI service");
        }
        
        var predictionStatusResults = await askAiClient.CallStatusPrediction(apiStatusInputDto);
        if (predictionStatusResults == null || predictionStatusResults.Percentages.Count != existingPrediction.DriverPredictions.Count)
        {
            throw new Exception("Invalid status prediction response from AI service");
        }
        
        var processedStatusDtos = ProcessStatusPredictionGetDtos(predictionStatusResults, 
            driverCodeToId, 
            constructorCodeToId);
        
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            foreach (var driverPrediction in existingPrediction.DriverPredictions)
            {
                var prediction = predictionResults.Predictions.FirstOrDefault(p => p.Input.DriverCode == driverPrediction.Driver.Code && p.Input.ConstructorCode == driverPrediction.Constructor.Code);
                if (prediction == null)
                {
                    throw new Exception("Prediction for driver-constructor pair not found");
                }
                driverPrediction.FinalPosition = prediction.PredictedFinalPosition;
                var statusPrediction = processedStatusDtos.FirstOrDefault(p => p.DriverId == driverPrediction.DriverId && p.ConstructorId == driverPrediction.ConstructorId);
                if (statusPrediction == null)
                {
                    throw new Exception("Status prediction for driver-constructor pair not found");
                }
                driverPrediction.Crashed = statusPrediction.Crashed;
            }
            // Move All Crashed to the bottom and reorder final positions accordingly
            var orderedDriverPredictions = existingPrediction.DriverPredictions
                .OrderBy(dp => dp.Crashed) // False (not crashed) comes before True (crashed)
                .ThenBy(dp => dp.FinalPosition) // Then order by final position
                .ToList();
            for (int i = 0; i < orderedDriverPredictions.Count; i++)
            {
                orderedDriverPredictions[i].FinalPosition = i + 1;
            }
            
            user.AskAiCredits -= 1;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to deduct AI credit");
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return existingPrediction.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating main race prediction as addition: {ex.Message}");

            
            throw;
        }
    }

    public async Task<List<Dtos.Get.PickableDriverGetDto>> GetMlPickableDriversAsync(AskAiClient.PredictionType predictionType)
    {
        var drivers = await askAiClient.GetPickableDriversAsync(predictionType);
        if (drivers == null)
        {
            throw new Exception("Failed to get pickable drivers from AI service");
        }

        var localDrivers = await staticDataRepository.GetAllDriversAsync();
        
        // Take only drivers that exist in both lists via driverRef / Code
        var pickableDrivers = localDrivers.Where(d => drivers.Any(ad => ad.DriverRef == d.Code)).ToList();
        return pickableDrivers.Select(AskAiDtoMapper.MapDriverToMlPickableDriverGetDto).ToList();
    }

    public async Task<List<Dtos.Get.PickableCircuitGetDto>> GetMlPickableCircuitsAsync(
        AskAiClient.PredictionType predictionType)
    {
        var circuits = await askAiClient.GetPickableCircuitsAsync(predictionType);
        if (circuits == null)
        {
            throw new Exception("Failed to get pickable circuits from AI service");
        }
        var localCircuits = await staticDataRepository.GetAllCircuitsAsync();
        
        // Take only circuits that exist in both lists via circuitRef / Code
        var pickableCircuits = localCircuits.Where(c => circuits.Any(ac => ac.CircuitRef == c.Code)).ToList();
        return pickableCircuits.Select(AskAiDtoMapper.MapCircuitToMlPickableCircuitGetDto).ToList();
    }

    public async Task<List<Dtos.Get.PickableConstructorGetDto>> GetMlPickableConstructorsAsync(
        AskAiClient.PredictionType predictionType)
    {
        var constructors = await askAiClient.GetPickableConstructorsAsync(predictionType);
        if (constructors == null)
        {
            throw new Exception("Failed to get pickable constructors from AI service");
        }
        var localConstructors = await staticDataRepository.GetAllConstructorsAsync();
        // Take only constructors that exist in both lists via constructorRef / Code
        var pickableConstructors = localConstructors.Where(c => constructors.Any(ac => ac.ConstructorRef == c.Code)).ToList();
        return pickableConstructors.Select(AskAiDtoMapper.MapConstructorToMlPickableConstructorGetDto).ToList();
    }

    #region  validate input existence for local database

    private async Task<(
    Dictionary<int, string> driverIdToCode,
    Dictionary<string, int> driverCodeToId,
    Dictionary<int, string> constructorIdToCode,
    Dictionary<string, int> constructorCodeToId,
    string circuitCode)> ValidateInputExistenceForLocalDatabaseCore(int circuitId, IEnumerable<int> drivers, IEnumerable<int> constructors)
    {
        var circuit = await staticDataRepository.GetCircuitByIdAsync(circuitId);
        if (circuit == null)
        {
            throw new NotFoundException($"Circuit with id {circuitId} not found");
        }

        var driverIdToCode = new Dictionary<int, string>();
        var driverCodeToId = new Dictionary<string, int>();
        var constructorIdToCode = new Dictionary<int, string>();
        var constructorCodeToId = new Dictionary<string, int>();

        var driverList = drivers.Distinct().ToList();
        var constructorList = constructors.Distinct().ToList();

        foreach (var driverId in driverList)
        {
            var driver = await staticDataRepository.GetDriverByIdAsync(driverId);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with id {driverId} not found");
            }
            driverIdToCode[driverId] = driver.Code;
            driverCodeToId[driver.Code] = driverId;
        }
        foreach (var constructorId in constructorList)
        {
            var constructor = await staticDataRepository.GetConstructorByIdAsync(constructorId);
            if (constructor == null)
            {
                throw new NotFoundException($"Constructor with id {constructorId} not found");
            }
            constructorIdToCode[constructorId] = constructor.Code;
            constructorCodeToId[constructor.Code] = constructorId;
        }

        return (driverIdToCode, driverCodeToId, constructorIdToCode, constructorCodeToId, circuit.Code);
    }

    // thin wrappers to preserve existing signatures and minimize call-site changes
    private Task<(
        Dictionary<int, string> driverIdToCode,
        Dictionary<string, int> driverCodeToId,
        Dictionary<int, string> constructorIdToCode,
        Dictionary<string, int> constructorCodeToId,
        string circuitCode
        )> ValidateInputExistenceForLocalDatabase(Dtos.Create.MainRacePredictionCreateAsNewDto mainRacePredictionCreateAsNewDto)
    {
        var drivers = mainRacePredictionCreateAsNewDto.Entries.Select(e => e.DriverId);
        var constructors = mainRacePredictionCreateAsNewDto.Entries.Select(e => e.ConstructorId);
        return ValidateInputExistenceForLocalDatabaseCore(mainRacePredictionCreateAsNewDto.CircuitId, drivers, constructors);
    }

    private Task<(
        Dictionary<int, string> driverIdToCode,
        Dictionary<string, int> driverCodeToId,
        Dictionary<int, string> constructorIdToCode,
        Dictionary<string, int> constructorCodeToId,
        string circuitCode
        )> ValidateInputExistenceForLocalDatabase(Dtos.Create.QualifyingPredictionCreateDto qualifyingPredictionCreateDto)
    {
        var drivers = qualifyingPredictionCreateDto.Entries.Select(e => e.DriverId);
        var constructors = qualifyingPredictionCreateDto.Entries.Select(e => e.ConstructorId);
        return ValidateInputExistenceForLocalDatabaseCore(qualifyingPredictionCreateDto.CircuitId, drivers, constructors);
    }

    private Task<(
        Dictionary<int, string> driverIdToCode,
        Dictionary<string, int> driverCodeToId,
        Dictionary<int, string> constructorIdToCode,
        Dictionary<string, int> constructorCodeToId,
        string circuitCode
        )> ValidateInputExistenceForLocalDatabase(Dtos.Create.StatusPredictionCreateDto statusPredictionCreateDto)
    {
        var drivers = statusPredictionCreateDto.Entries.Select(e => e.DriverId);
        var constructors = statusPredictionCreateDto.Entries.Select(e => e.ConstructorId);
        return ValidateInputExistenceForLocalDatabaseCore(statusPredictionCreateDto.CircuitId, drivers, constructors);
    }

    #endregion

    #region validate input existence for ml model

    private async Task ValidateInputExistenceForMlModelCore(string circuitRef, IEnumerable<string> driverRefs, IEnumerable<string> constructorRefs, AskAiClient.PredictionType predictionType)
    {
        var availableCircuits = await askAiClient.GetPickableCircuitsAsync(predictionType);
        if (availableCircuits == null || availableCircuits.All(c => c.CircuitRef != circuitRef))
        {
            throw new NotFoundException($"Circuit with code {circuitRef} not found in ML model");
        }
        
        var availableDrivers = await askAiClient.GetPickableDriversAsync(predictionType);
        // Every driverRef must exist in availableDrivers
        foreach (var driverCode in driverRefs)
        {
            if (availableDrivers == null || availableDrivers.All(d => d.DriverRef != driverCode))
            {
                throw new NotFoundException($"Driver with code {driverCode} not found in ML model");
            }
        }
        
        var availableConstructors = await askAiClient.GetPickableConstructorsAsync(predictionType);
        // Every constructorRef must exist in availableConstructors
        foreach (var constructorCode in constructorRefs)
        {
            if (availableConstructors == null || availableConstructors.All(c => c.ConstructorRef != constructorCode))
            {
                throw new NotFoundException($"Constructor with code {constructorCode} not found in ML model");
            }
        }
    }

    private Task ValidateInputExistenceForMlModel(List<Dtos.Api.MainRacePredictInputDto> inputDtos)
    {
        var driverRefs = inputDtos.Select(e => e.DriverCode);
        var constructorRefs = inputDtos.Select(e => e.ConstructorCode);
        var circuitRef = inputDtos.First().CircuitCode;
        return ValidateInputExistenceForMlModelCore(circuitRef, driverRefs, constructorRefs , AskAiClient.PredictionType.MainRace);
    }
    
    private Task ValidateInputExistenceForMlModel(List<Dtos.Api.QualifyingPredictInputDto> inputDtos)
    {
        var driverRefs =   inputDtos.Select(e => e.DriverCode);
        var constructorRefs =  inputDtos.Select(e => e.ConstructorCode);
        var circuitRef =  inputDtos.First().CircuitCode;
        return ValidateInputExistenceForMlModelCore(circuitRef, driverRefs, constructorRefs, AskAiClient.PredictionType.Qualifying);
    }
    
    private Task ValidateInputExistenceForMlModel(List<Dtos.Api.StatusPredictInputDto> inputDtos)
    {
        var driverRefs =   inputDtos.Select(e => e.DriverCode);
        var constructorRefs =  inputDtos.Select(e => e.ConstructorCode);
        var circuitRef =  inputDtos.First().CircuitCode;
        return ValidateInputExistenceForMlModelCore(circuitRef, driverRefs, constructorRefs, AskAiClient.PredictionType.Status);
    }
    #endregion
    private void ValidateUniqueDriverConstructorPair(List<DriverPredictionInputCreateDto> entries)
    {
        var uniquePairs = new HashSet<string>();
        foreach (var entry in entries)
        {
            var pairKey = $"{entry.DriverId}-{entry.ConstructorId}";
            if (!uniquePairs.Add(pairKey))
            {
                throw new InvalidOperationException($"Duplicate driver-constructor pair found: {pairKey}");
            }
        }
    }

    private List<ProcessedMainRacePredictionGetDto> ProcessMainRacePredictionGetDtos( Dtos.Api.MainRacePredictResponseDto responseDto, Dictionary<string, int> driverCodeToId, Dictionary<string, int> constructorCodeToId)
    {
        var results = new List<ProcessedMainRacePredictionGetDto>();
        foreach (var entry in responseDto.Predictions)
        {
            if (!driverCodeToId.ContainsKey(entry.Input.DriverCode) || !constructorCodeToId.ContainsKey(entry.Input.ConstructorCode))
            {
                throw new Exception("Driver or Constructor code not found in mapping");
            }
            results.Add(new ProcessedMainRacePredictionGetDto
            {
                DriverId = driverCodeToId[entry.Input.DriverCode],
                ConstructorId = constructorCodeToId[entry.Input.ConstructorCode],
                GridPosition = entry.Input.QualificationPosition,
                FinalPosition = entry.PredictedFinalPosition,
            });
        }
        return results;
    }
    
    private List<ProccessedQualifyingPredictionDto> ProcessQualifyingPredictionGetDtos( Dtos.Api.QualifyingPredictResponseDto responseDto, Dictionary<string, int> driverCodeToId, Dictionary<string, int> constructorCodeToId)
    {
        var results = new List<ProccessedQualifyingPredictionDto>();
        foreach (var entry in responseDto.Predictions)
        {
            if (!driverCodeToId.ContainsKey(entry.Input.DriverCode) || !constructorCodeToId.ContainsKey(entry.Input.ConstructorCode))
            {
                throw new Exception("Driver or Constructor code not found in mapping");
            }
            results.Add(new ProccessedQualifyingPredictionDto
            {
                DriverId = driverCodeToId[entry.Input.DriverCode],
                ConstructorId = constructorCodeToId[entry.Input.ConstructorCode],
                GridPosition = entry.PredictedFinalPosition
            });
        }
        return results;
    }
    private List<ProcessedStatusGetDto> ProcessStatusPredictionGetDtos( Dtos.Api.StatusPredictResponseDto responseDto, Dictionary<string, int> driverCodeToId, Dictionary<string, int> constructorCodeToId)
    {
        var results = new List<ProcessedStatusGetDto>();
        foreach (var entry in responseDto.Percentages)
        {
            if (!driverCodeToId.ContainsKey(entry.Input.DriverCode) || !constructorCodeToId.ContainsKey(entry.Input.ConstructorCode))
            {
                throw new Exception("Driver or Constructor code not found in mapping");
            }
            // PredictedDnfPercentage is from 0 to 1, do random to determine if crashed
            var random = new Random();
            var crashed = random.NextDouble()*100 < entry.PredictedDnfPercentage;
            
            results.Add(new ProcessedStatusGetDto
            {
                DriverId = driverCodeToId[entry.Input.DriverCode],
                ConstructorId = constructorCodeToId[entry.Input.ConstructorCode],
                Crashed = crashed
            });
        }
        return results;
    }
    
    private void ValidatePredictionCountCap (int entryCount)
    {
        var maxEntries = configuration.GetValue<int>("CoreGameplaySettings:AskAiSettings:MaxEntryCountInPrediction"); 
        if (entryCount > maxEntries)
        {
            throw new InvalidOperationException($"Prediction entry count exceeds the maximum allowed of {maxEntries}");
        }
        if(entryCount <= 1)
        {
            throw new InvalidOperationException("Prediction entry count must be greater than one");
        }
    }

    private async Task<(
        Dictionary<int, string> driverIdToCode,
        Dictionary<string, int> driverCodeToId,
        Dictionary<int, string> constructorIdToCode,
        Dictionary<string, int> constructorCodeToId)> CreateDictionariesExtensionFromPrediction(Prediction prediction)
    {
        var driverIdToCode = new Dictionary<int, string>();
        var driverCodeToId = new Dictionary<string, int>();
        var constructorIdToCode = new Dictionary<int, string>();
        var constructorCodeToId = new Dictionary<string, int>();

        foreach (var driverPrediction in prediction.DriverPredictions)
        {
            driverIdToCode[driverPrediction.DriverId] = driverPrediction.Driver.Code;
            driverCodeToId[ driverPrediction.Driver.Code] = driverPrediction.DriverId;
            
            constructorIdToCode[driverPrediction.ConstructorId] = driverPrediction.Constructor.Code;
            constructorCodeToId[driverPrediction.Constructor.Code] = driverPrediction.ConstructorId;
        }
        return (driverIdToCode, driverCodeToId, constructorIdToCode, constructorCodeToId);
    }
}