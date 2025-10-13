using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AskAiModule.Dtos.Create;
using F1Fantasy.Modules.AskAiModule.Dtos.Get;
using F1Fantasy.Modules.AskAiModule.Dtos.Mapper;
using F1Fantasy.Modules.AskAiModule.Extensions;
using F1Fantasy.Modules.AskAiModule.Repositories.Interfaces;
using F1Fantasy.Modules.AskAiModule.Services.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace F1Fantasy.Modules.AskAiModule.Services.Implementations;

public class AskAiService(UserManager<ApplicationUser> userManager, IAskAiRepository askAiRepository, AskAiClient askAiClient, WooF1Context context, IStaticDataRepository staticDataRepository) : IAskAIService
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
        Dtos.Create.MainRacePredictionCreateDto mainRacePredictionCreateDto)
    {
       // TODO 
       return 0;
    }
    
     public async Task<int> MadeMainRacePredictionAsNewPredictionAsync(int userId,
        Dtos.Create.MainRacePredictionCreateDto mainRacePredictionCreateDto)
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

        var (driverCodeToId, constructorCodeToId) = ValidateInputExistence(mainRacePredictionCreateDto);      
        ValidateUniqueDriverConstructorPair(mainRacePredictionCreateDto.Entries);
        
        var apiInputDto = AskAiDtoMapper.MapMainRaceCreateDtoToApiInputDto(mainRacePredictionCreateDto);
        var apiStatusInputDto = AskAiDtoMapper.MapMainRaceCreateDtoToStatusApiInputDto(mainRacePredictionCreateDto);
        
        var predictionResults = await askAiClient.CallMainRacePrediction(apiInputDto);
        if (predictionResults == null || predictionResults.Predictions.Count != mainRacePredictionCreateDto.Entries.Count)
        {
            throw new Exception("Invalid main race prediction response from AI service");
        }
        
        var predictionStatusResults = await askAiClient.CallStatusPrediction(apiStatusInputDto);
        if (predictionStatusResults == null || predictionStatusResults.Percentages.Count != mainRacePredictionCreateDto.Entries.Count)
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
            var newPrediction = AskAiDtoMapper.MapMainRaceCreateDtoToPrediction(userId, mainRacePredictionCreateDto);
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
            Console.WriteLine($"Error creating prediction: {ex.Message}");

            
            throw;
        }
    }
     
    private (Dictionary<string, int> driverCodeToId, Dictionary<string, int> constructorCodeToId) ValidateInputExistence(Dtos.Create.MainRacePredictionCreateDto mainRacePredictionCreateDto)
    {
        var circuit = staticDataRepository.GetCircuitByCodeAsync(mainRacePredictionCreateDto.CircuitCode);
        if (circuit == null)
        {
            throw new NotFoundException($"Circuit with code {mainRacePredictionCreateDto.CircuitCode} not found");
        }

        var driverCodeToId = new Dictionary<string, int>();
        var constructorCodeToId = new Dictionary<string, int>();

        List<string> drivers = mainRacePredictionCreateDto.Entries.Select(e => e.DriverCode).ToList();
        List<string> constructors = mainRacePredictionCreateDto.Entries.Select(e => e.ConstructorCode).ToList();

        foreach (var driverCode in drivers)
        {
            var driver = staticDataRepository.GetDriverByCodeAsync(driverCode);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with code {driverCode} not found");
            }
            driverCodeToId[driverCode] = driver.Id;
        }
        foreach (var constructorCode in constructors)
        {
            var constructor = staticDataRepository.GetConstructorByCodeAsync(constructorCode);
            if (constructor == null)
            {
                throw new NotFoundException($"Constructor with code {constructorCode} not found");
            }
            constructorCodeToId[constructorCode] = constructor.Id;
        }

        return (driverCodeToId, constructorCodeToId);
    }
    
    private (Dictionary<string, int> driverCodeToId, Dictionary<string, int> constructorCodeToId) ValidateInputExistence(Dtos.Create.QualifyingPredictionCreateDto qualifyingPredictionCreateDto)
    {
        var circuit = staticDataRepository.GetCircuitByCodeAsync(qualifyingPredictionCreateDto.CircuitCode);
        if (circuit == null)
        {
            throw new NotFoundException($"Circuit with code {qualifyingPredictionCreateDto.CircuitCode} not found");
        }

        var driverCodeToId = new Dictionary<string, int>();
        var constructorCodeToId = new Dictionary<string, int>();

        List<string> drivers = qualifyingPredictionCreateDto.Entries.Select(e => e.DriverCode).ToList();
        List<string> constructors = qualifyingPredictionCreateDto.Entries.Select(e => e.ConstructorCode).ToList();

        foreach (var driverCode in drivers)
        {
            var driver = staticDataRepository.GetDriverByCodeAsync(driverCode);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with code {driverCode} not found");
            }
            driverCodeToId[driverCode] = driver.Id;
        }
        foreach (var constructorCode in constructors)
        {
            var constructor = staticDataRepository.GetConstructorByCodeAsync(constructorCode);
            if (constructor == null)
            {
                throw new NotFoundException($"Constructor with code {constructorCode} not found");
            }
            constructorCodeToId[constructorCode] = constructor.Id;
        }

        return (driverCodeToId, constructorCodeToId);
    }
    
    private (Dictionary<string, int> driverCodeToId, Dictionary<string, int> constructorCodeToId) ValidateInputExistence(Dtos.Create.StatusPredictionCreateDto statusPredictionCreateDto)
    {
        var circuit = staticDataRepository.GetCircuitByCodeAsync(statusPredictionCreateDto.CircuitCode);
        if (circuit == null)
        {
            throw new NotFoundException($"Circuit with code {statusPredictionCreateDto.CircuitCode} not found");
        }

        var driverCodeToId = new Dictionary<string, int>();
        var constructorCodeToId = new Dictionary<string, int>();

        List<string> drivers = statusPredictionCreateDto.Entries.Select(e => e.DriverCode).ToList();
        List<string> constructors = statusPredictionCreateDto.Entries.Select(e => e.ConstructorCode).ToList();

        foreach (var driverCode in drivers)
        {
            var driver = staticDataRepository.GetDriverByCodeAsync(driverCode);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with code {driverCode} not found");
            }
            driverCodeToId[driverCode] = driver.Id;
        }
        foreach (var constructorCode in constructors)
        {
            var constructor = staticDataRepository.GetConstructorByCodeAsync(constructorCode);
            if (constructor == null)
            {
                throw new NotFoundException($"Constructor with code {constructorCode} not found");
            }
            constructorCodeToId[constructorCode] = constructor.Id;
        }

        return (driverCodeToId, constructorCodeToId);
    }

    private void ValidateUniqueDriverConstructorPair(List<DriverPredictionInputCreateDto> entries)
    {
        var uniquePairs = new HashSet<string>();
        foreach (var entry in entries)
        {
            var pairKey = $"{entry.DriverCode}-{entry.ConstructorCode}";
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
            var crashed = random.NextDouble() < entry.PredictedDnfPercentage;
            
            results.Add(new ProcessedStatusGetDto
            {
                DriverId = driverCodeToId[entry.Input.DriverCode],
                ConstructorId = constructorCodeToId[entry.Input.ConstructorCode],
                Crashed = crashed
            });
        }
        return results;
    }
}