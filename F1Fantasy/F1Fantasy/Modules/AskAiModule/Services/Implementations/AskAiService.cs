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
            circuitCode) = await ValidateInputExistence(mainRacePredictionCreateAsNewDto);      
        ValidateUniqueDriverConstructorPair(mainRacePredictionCreateAsNewDto.Entries);
        ValidatePredictionCountCap(mainRacePredictionCreateAsNewDto.Entries.Count);
        
        var apiInputDto = AskAiDtoMapper.MapMainRaceCreateAsNewDtoToApiInputDto(mainRacePredictionCreateAsNewDto, driverIdToCode, constructorIdToCode, circuitCode);
        var apiStatusInputDto = AskAiDtoMapper.MapMainRaceCreateDtoToStatusApiInputDto(mainRacePredictionCreateAsNewDto, driverIdToCode, constructorIdToCode, circuitCode);
        
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
            circuitCode) = await ValidateInputExistence(qualifyingPredictionCreateDto);      
        ValidateUniqueDriverConstructorPair(qualifyingPredictionCreateDto.Entries);
        ValidatePredictionCountCap(qualifyingPredictionCreateDto.Entries.Count);
        
        var apiInputDto = AskAiDtoMapper.MapQualifyingCreateDtoToApiInputDto(qualifyingPredictionCreateDto, driverIdToCode, constructorIdToCode, circuitCode);
        
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
        
        var apiInputDtos = AskAiDtoMapper.MapMainRaceCreateAsAdditionToApiInputDto(mainRacePredictionCreateAsAdditionDto, existingPrediction);
        
        var predictionResults = await askAiClient.CallMainRacePrediction(apiInputDtos);
        if (predictionResults == null || predictionResults.Predictions.Count != existingPrediction.DriverPredictions.Count)
        {
            throw new Exception("Invalid main race prediction response from AI service");
        }
        
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
    
    private async Task<(
        Dictionary<int, string> driverIdToCode,
        Dictionary<string, int> driverCodeToId,
        Dictionary<int, string> constructorIdToCode,
        Dictionary<string, int> constructorCodeToId,
        string circuitCode
        )> ValidateInputExistence(Dtos.Create.MainRacePredictionCreateAsNewDto mainRacePredictionCreateAsNewDto)
    {
        var circuit = await staticDataRepository.GetCircuitByIdAsync(mainRacePredictionCreateAsNewDto.CircuitId);
        if (circuit == null)
        {
            throw new NotFoundException($"Circuit with id {mainRacePredictionCreateAsNewDto.CircuitId} not found");
        }

        var driverIdToCode = new Dictionary<int, string>();
        var driverCodeToId = new Dictionary<string, int>();
        var constructorIdToCode = new Dictionary<int, string>();
        var constructorCodeToId = new Dictionary<string, int>();

        List<int> drivers = mainRacePredictionCreateAsNewDto.Entries.Select(e => e.DriverId).ToList();
        List<int> constructors = mainRacePredictionCreateAsNewDto.Entries.Select(e => e.ConstructorId).ToList();

        foreach (var driverId in drivers)
        {
            var driver = await staticDataRepository.GetCircuitByIdAsync(driverId);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with code {driverId} not found");
            }
            driverIdToCode[driverId] = driver.Code;
            driverCodeToId[driver.Code] = driverId;
        }
        foreach (var constructorId in constructors)
        {
            var constructor = await staticDataRepository.GetConstructorByIdAsync(constructorId);
            if (constructor == null)
            {
                throw new NotFoundException($"Constructor with code {constructorId} not found");
            }
            constructorIdToCode[constructorId] = constructor.Code;
            constructorCodeToId[constructor.Code] = constructorId;
        }

        return (driverIdToCode, driverCodeToId, constructorIdToCode, constructorCodeToId, circuit.Code);
    }
    
    private async Task<(
        Dictionary<int, string> driverIdToCode,
        Dictionary<string, int> driverCodeToId,
        Dictionary<int, string> constructorIdToCode,
        Dictionary<string, int> constructorCodeToId,
        string circuitCode
        )> ValidateInputExistence(Dtos.Create.QualifyingPredictionCreateDto qualifyingPredictionCreateDto)
    {
        var circuit = await staticDataRepository.GetCircuitByIdAsync(qualifyingPredictionCreateDto.CircuitId);
        if (circuit == null)
        {
            throw new NotFoundException($"Circuit with id {qualifyingPredictionCreateDto.CircuitId} not found");
        }

        var driverIdToCode = new Dictionary<int, string>();
        var driverCodeToId = new Dictionary<string, int>();
        var constructorIdToCode = new Dictionary<int, string>();
        var constructorCodeToId = new Dictionary<string, int>();

        List<int> drivers = qualifyingPredictionCreateDto.Entries.Select(e => e.DriverId).ToList();
        List<int> constructors = qualifyingPredictionCreateDto.Entries.Select(e => e.ConstructorId).ToList();

        foreach (var driverId in drivers)
        {
            var driver = await staticDataRepository.GetDriverByIdAsync(driverId);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with id {driverId} not found");
            }
            driverIdToCode[driverId] = driver.Code;
            driverCodeToId[driver.Code] = driverId;
        }
        foreach (var constructorId in constructors)
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
    
    private async Task<(
        Dictionary<int, string> driverIdToCode,
        Dictionary<string, int> driverCodeToId,
        Dictionary<int, string> constructorIdToCode,
        Dictionary<string, int> constructorCodeToId,
        string circuitCode
        )> ValidateInputExistence(Dtos.Create.StatusPredictionCreateDto statusPredictionCreateDto)
    {
        var circuit = await staticDataRepository.GetCircuitByIdAsync(statusPredictionCreateDto.CircuitId);
        if (circuit == null)
        {
            throw new NotFoundException($"Circuit with id {statusPredictionCreateDto.CircuitId} not found");
        }

        var driverIdToCode = new Dictionary<int, string>();
        var driverCodeToId = new Dictionary<string, int>();
        var constructorIdToCode = new Dictionary<int, string>();
        var constructorCodeToId = new Dictionary<string, int>();

        List<int> drivers = statusPredictionCreateDto.Entries.Select(e => e.DriverId).ToList();
        List<int> constructors = statusPredictionCreateDto.Entries.Select(e => e.ConstructorId).ToList();

        foreach (var driverId in drivers)
        {
            var driver = await staticDataRepository.GetCircuitByIdAsync(driverId);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with code {driverId} not found");
            }
            driverIdToCode[driverId] = driver.Code;
            driverCodeToId[driver.Code] = driverId;
        }
        foreach (var constructorId in constructors)
        {
            var constructor = await staticDataRepository.GetConstructorByIdAsync(constructorId);
            if (constructor == null)
            {
                throw new NotFoundException($"Constructor with code {constructorId} not found");
            }
            constructorIdToCode[constructorId] = constructor.Code;
            constructorCodeToId[constructor.Code] = constructorId;
        }

        return (driverIdToCode, driverCodeToId, constructorIdToCode, constructorCodeToId, circuit.Code);
    }

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
}