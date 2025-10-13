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
            PredictYear = prediction.PredictYear,
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
            PredictYear = prediction.PredictYear,
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

    public static List<Api.MainRacePredictInputDto> MapMainRaceCreateDtoToApiInputDto(
        Dtos.Create.MainRacePredictionCreateDto createDto)
    {
        return createDto.Entries.Select(dto => new Dtos.Api.MainRacePredictInputDto
        {
            DriverCode = dto.DriverCode,
            ConstructorCode = dto.ConstructorCode,
            CircuitCode = createDto.CircuitCode,
            Rain = createDto.Rain,
            Laps = createDto.Laps,
            RaceDate = createDto.RaceDate,
        }).ToList();
    }
            
    public static Prediction MapMainRaceCreateDtoToPrediction( int userId,
        Dtos.Create.MainRacePredictionCreateDto createDto)
    {
        return new Prediction
        {
            DatePredicted = DateOnly.FromDateTime(DateTime.UtcNow),
            PredictYear = createDto.RaceDate.Year,
            Rain = createDto.Rain,
            UserId = userId
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
    public static List<Api.QualifyingPredictInputDto> MapQualifyingCreateDtoToApiInputDto(Dtos.Create.QualifyingPredictionCreateDto createDto)
    {
        return createDto.Entries.Select(dto => new Dtos.Api.QualifyingPredictInputDto
        {
            DriverCode = dto.DriverCode,
            ConstructorCode = dto.ConstructorCode,
            CircuitCode = createDto.CircuitCode,
            RaceDate = createDto.RaceDate,
        }).ToList();
    }
    
    public static List<Api.StatusPredictInputDto> MapStatusCreateDtoToApiInputDto(Dtos.Create.StatusPredictionCreateDto createDto)
    {
        return createDto.Entries.Select(dto => new Dtos.Api.StatusPredictInputDto
        {
            DriverCode = dto.DriverCode,
            ConstructorCode = dto.ConstructorCode,
            CircuitCode = createDto.CircuitCode,
            Rain = createDto.Rain,
            RaceDate = createDto.RaceDate,
        }).ToList();
    }
    
    public static List<Api.StatusPredictInputDto> MapMainRaceCreateDtoToStatusApiInputDto(Dtos.Create.MainRacePredictionCreateDto createDto)
    {
        return createDto.Entries.Select(dto => new Dtos.Api.StatusPredictInputDto
        {
            DriverCode = dto.DriverCode,
            ConstructorCode = dto.ConstructorCode,
            CircuitCode = createDto.CircuitCode,
            Rain = createDto.Rain,
            RaceDate = createDto.RaceDate,
        }).ToList();
    }
}