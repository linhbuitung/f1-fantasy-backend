using F1Fantasy.Exceptions;
using F1Fantasy.Modules.StatisticModule.Dtos.Get;
using F1Fantasy.Modules.StatisticModule.Dtos.Mapper;
using F1Fantasy.Modules.StatisticModule.Helpers;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;

namespace F1Fantasy.Modules.StatisticModule.Services.Implementations;

public class RaceStatisticService(IStatisticRepository statisticRepository) : IRaceStatisticService
{
    public async Task<RaceStatisticDto> GetRaceStatisticByIdAsync(int raceId)
    {
        var race = await statisticRepository.GetRaceIncludeRaceEntriesByIdAsync(raceId);
        if (race == null)
        {
            throw new NotFoundException("Race statistic not found");
        }

        var raceStatisticDto = StatisticDtoMapper.MapToInitialRaceStatisticDto(race);
        
        // For each unique constructor, get the list of all driver position of them in this race,
        // then calculate the total points gained by this constructor, and map to ConstructorInRaceStatisticDto
        var constructorDtos = new List<ConstructorInRaceStatisticDto>();
        var uniqueConstructorIds = race.RaceEntries
            .Select(re => re.Constructor.Id)
            .Distinct();

        foreach (var constructorId in uniqueConstructorIds)
        {
            var driverPositions = race.RaceEntries
                .Where(re => re.Constructor.Id == constructorId)
                .Select(re => re.Position)
                .ToList();
            var pointsGained = StatisticHelper.CalculateConstructorPoints(driverPositions);
            var constructor = race.RaceEntries
                .First(re => re.Constructor.Id == constructorId)
                .Constructor;
            var constructorDto = StatisticDtoMapper.MapToConstructorInRaceStatisticDto(constructor, pointsGained);
            constructorDtos.Add(constructorDto);
        }
        raceStatisticDto.ConstructorsStatistics = constructorDtos;
        return raceStatisticDto;
    }

}