using System.Diagnostics;
using F1Fantasy.Core.Common;
using F1Fantasy.Modules.AskAiModule.Dtos.Create;
using F1Fantasy.Modules.AskAiModule.Dtos.Get;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Mapper;

public class AskAiDtoMapper
{
    public static PredictionGetDto MapPredictionToGetDtoDetail(Prediction prediction)
    {
        return new PredictionGetDto
        {
            Id = prediction.Id,
            DatePredicted = prediction.DatePredicted,
            RaceDate = prediction.RaceDate,
            QualifyingDate = prediction.QualifyingDate,
            Rain = prediction.Rain,
            UserId = prediction.UserId,
            // if there is any driver prediction entity linked with a grid position not null, then set true
            IsQualifyingCalculated = prediction.DriverPredictions.Any(dp => dp.GridPosition != null),
            // if there is any constructor prediction entity linked with a final position not null, then set true
            IsRaceCalculated = prediction.DriverPredictions.Any(dp => dp.FinalPosition != null),
            Circuit = new CircuitInPredictionGetDto()
            {
                Id = prediction.Circuit.Id,
                CircuitName = prediction.Circuit.CircuitName,
                Code = prediction.Circuit.Code,
                Latitude = prediction.Circuit.Latitude,
                Longitude = prediction.Circuit.Longitude,
                Locality = prediction.Circuit.Locality,
                CountryId = prediction.Circuit.CountryId,
                ImgUrl = prediction.Circuit.ImgUrl
            },
            DriverPredictions = prediction.DriverPredictions.Select(dp => new DriverPredictionGetDto()
            {
                Id = dp.Id,
                GridPosition = dp.GridPosition,
                FinalPosition = dp.FinalPosition,
                Crashed = dp.Crashed,
                PredictionId = dp.PredictionId,
                Driver = new DriverInDriverPreditionGetDto()
                {
                    Id = dp.Driver.Id,
                    GivenName = dp.Driver.GivenName,
                    FamilyName = dp.Driver.FamilyName,
                    DateOfBirth = dp.Driver.DateOfBirth,
                    Code = dp.Driver.Code,
                    ImgUrl = dp.Driver.ImgUrl
                },
                Constructor = new ConstructorInDriverPredictionGetDto()
                {
                    Id = dp.Constructor.Id,
                    Name = dp.Constructor.Name,
                    Code = dp.Constructor.Code,
                    ImgUrl = dp.Constructor.ImgUrl,
                    }
                }).ToList()
        };
    }

    public static PredictionGetDto MapPredictionToGetDtoMinimal(Prediction prediction)
    {
        return new PredictionGetDto
        {
            Id = prediction.Id,
            DatePredicted = prediction.DatePredicted,
            RaceDate = prediction.RaceDate,
            QualifyingDate = prediction.QualifyingDate,
            Rain = prediction.Rain,
            UserId = prediction.UserId,
            // if there is any driver prediction entity linked with a grid position not null, then set true
            IsQualifyingCalculated = prediction.DriverPredictions.Any(dp => dp.GridPosition != null),
            // if there is any constructor prediction entity linked with a final position not null, then set true
            IsRaceCalculated = prediction.DriverPredictions.Any(dp => dp.FinalPosition != null),
            Circuit = new CircuitInPredictionGetDto()
            {
                Id = prediction.Circuit.Id,
                CircuitName = prediction.Circuit.CircuitName,
                Code = prediction.Circuit.Code,
                Latitude = prediction.Circuit.Latitude,
                Longitude = prediction.Circuit.Longitude,
                Locality = prediction.Circuit.Locality,
                CountryId = prediction.Circuit.CountryId,
                ImgUrl = prediction.Circuit.ImgUrl
            },
            DriverPredictions = null
        };
    }

    public static List<Api.MainRacePredictInputDto> MapMainRaceCreateAsNewDtoToApiInputDto(
        Dtos.Create.MainRacePredictionCreateAsNewDto createAsNewDto, Dictionary<int, string> driverIdToCode, Dictionary<int, string> constructorIdToCode, string circuitCode)
    {
        return createAsNewDto.Entries.Select(dto => new Dtos.Api.MainRacePredictInputDto
        {
            QualificationPosition = dto.QualificationPosition ?? 0,
            DriverCode = driverIdToCode[dto.DriverId],
            ConstructorCode = constructorIdToCode[dto.ConstructorId],
            CircuitCode = circuitCode,
            Rain = createAsNewDto.Rain ? 1 : 0,
            Laps = createAsNewDto.Laps,
            RaceDate = DateOnly.FromDateTime(createAsNewDto.RaceDate),
        }).ToList();
    }
    
    public static List<Api.MainRacePredictInputDto> MapMainRaceCreateAsAdditionToApiInputDto(
        Dtos.Create.MainRacePredictionCreateAsAdditionDto createAsAdditionDto, Prediction prediction)
    {
        return prediction.DriverPredictions.Select(dp => new Dtos.Api.MainRacePredictInputDto
        {
            DriverCode = dp.Driver.Code,
            ConstructorCode = dp.Constructor.Code,
            CircuitCode = prediction.Circuit.Code,
            Rain = createAsAdditionDto.Rain ? 1 : 0,
            Laps = createAsAdditionDto.Laps,
            RaceDate = DateOnly.FromDateTime(createAsAdditionDto.RaceDate),
        }).ToList();
    }
            
    public static Prediction MapMainRaceCreateDtoToPrediction( int userId,
        Dtos.Create.MainRacePredictionCreateAsNewDto createAsNewDto)
    {
        return new Prediction
        {
            DatePredicted = DateOnly.FromDateTime(DateTime.UtcNow),
            RaceDate = createAsNewDto.RaceDate,
            QualifyingDate = createAsNewDto.QualifyingDate,
            Rain = createAsNewDto.Rain,
            CircuitId = createAsNewDto.CircuitId,
            UserId = userId
        };
    }
    
    public static Prediction MapQualifyingCreateDtoToPrediction( int userId,
        Dtos.Create.QualifyingPredictionCreateDto createDto)
    {
        return new Prediction
        {
            DatePredicted = DateOnly.FromDateTime(DateTime.UtcNow),
            QualifyingDate = createDto.QualifyingDate,
            Rain = false,
            UserId = userId,
            CircuitId = createDto.CircuitId
        };
    }
    
    public static List<DriverPrediction> MapApiResultToNewDriverPrediction( int predictionId,
        List<ProcessedMainRacePredictionGetDto> processedMainRacePredictionGetDto,  List<ProcessedStatusGetDto> processedStatusDtos)
    {
        var driverPredictions = new List<DriverPrediction>();
        foreach (var entryPrediction in processedMainRacePredictionGetDto)
        {
            // Find the matching status result for the driver and constructor
            var statusResult = processedStatusDtos.FirstOrDefault(s => s.DriverId == entryPrediction.DriverId && 
                                                                       s.ConstructorId == entryPrediction.ConstructorId);
            if (statusResult == null)
            {
                throw new Exception("Crash not found");
            }

            driverPredictions.Add(new DriverPrediction
            {
                PredictionId = predictionId,
                DriverId = entryPrediction.DriverId,
                ConstructorId = entryPrediction.ConstructorId,
                GridPosition = entryPrediction.GridPosition,
                FinalPosition = entryPrediction.FinalPosition,
                Crashed = statusResult.Crashed,
            });
            
        }
        return driverPredictions;
    }
    
    public static List<DriverPrediction> MapApiResultToNewDriverPrediction( int predictionId,
        List<ProccessedQualifyingPredictionDto> proccessedQualifyingPredictionDtos)
    {
        var driverPredictions = new List<DriverPrediction>();
        foreach (var entryPrediction in proccessedQualifyingPredictionDtos)
        {
            driverPredictions.Add(new DriverPrediction
            {
                PredictionId = predictionId,
                DriverId = entryPrediction.DriverId,
                ConstructorId = entryPrediction.ConstructorId,
                GridPosition = entryPrediction.GridPosition,
            });
            
        }
        return driverPredictions;
    }
    
    public static List<Api.QualifyingPredictInputDto> MapQualifyingCreateDtoToApiInputDto(
        Dtos.Create.QualifyingPredictionCreateDto createDto, 
        Dictionary<int, string> driverIdToCode, 
        Dictionary<int, string> constructorIdToCode, 
        string circuitCode)
    {
        return createDto.Entries.Select(dto => new Dtos.Api.QualifyingPredictInputDto
        {
            DriverCode = driverIdToCode[dto.DriverId],
            ConstructorCode = constructorIdToCode[dto.ConstructorId],
            CircuitCode = circuitCode,
            RaceDate = DateOnly.FromDateTime(createDto.QualifyingDate),
        }).ToList();
    }
    
    public static List<Api.StatusPredictInputDto> MapStatusCreateDtoToApiInputDto(
        Dtos.Create.StatusPredictionCreateDto createDto, 
        Dictionary<int, string> driverIdToCode, 
        Dictionary<int, string> constructorIdToCode, 
        string circuitCode)
    {
        return createDto.Entries.Select(dto => new Dtos.Api.StatusPredictInputDto
        {
            DriverCode = driverIdToCode[dto.DriverId],
            ConstructorCode = constructorIdToCode[dto.ConstructorId],
            CircuitCode = circuitCode,
            Rain = createDto.Rain ? 1 : 0,
            RaceDate = DateOnly.FromDateTime(createDto.RaceDate),
        }).ToList();
    }
    
    public static List<Api.StatusPredictInputDto> MapMainRaceCreateAsNewDtoToStatusApiInputDto(
        Dtos.Create.MainRacePredictionCreateAsNewDto createAsNewDto,         
        Dictionary<int, string> driverIdToCode, 
        Dictionary<int, string> constructorIdToCode, 
        string circuitCode)
    {
        return createAsNewDto.Entries.Select(dto => new Dtos.Api.StatusPredictInputDto
        {
            DriverCode = driverIdToCode[dto.DriverId],
            ConstructorCode = constructorIdToCode[dto.ConstructorId],
            CircuitCode = circuitCode,
            Rain = createAsNewDto.Rain ? 1 : 0,
            RaceDate = DateOnly.FromDateTime(createAsNewDto.RaceDate),
        }).ToList();
    }
    
    public static List<Api.StatusPredictInputDto> MapMainRaceCreateAsAdditionDtoToStatusApiInputDto(
        List<DriverPrediction> driverPredictions,
        Dtos.Create.MainRacePredictionCreateAsAdditionDto createAsNewDto,         
        Dictionary<int, string> driverIdToCode, 
        Dictionary<int, string> constructorIdToCode, 
        string circuitCode)
    {
        return driverPredictions.Select(dto => new Dtos.Api.StatusPredictInputDto
        {
            DriverCode = driverIdToCode[dto.DriverId],
            ConstructorCode = constructorIdToCode[dto.ConstructorId],
            CircuitCode = circuitCode,
            Rain = createAsNewDto.Rain ? 1 : 0,
            RaceDate = DateOnly.FromDateTime(createAsNewDto.RaceDate),
        }).ToList();
    }
}