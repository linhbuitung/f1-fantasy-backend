using F1Fantasy.Modules.CoreGameplayModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Dtos.Mapper;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;

namespace F1Fantasy.Modules.StatisticModule.Services.Implementations;

public class ConstructorStatisticService(IStatisticRepository statisticRepository, ICoreGameplayRepository coreGameplayRepository) : IConstructorStatisticService
{
    public async Task<List<Dtos.Get.ConstructorWithTotalFantasyPointScoredGetDto>> GetTopConstructorsInSeasonByTotalFantasyPointsAsync(int seasonId)
    {
        var constructors = await statisticRepository.GetAllConstructorsIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var constructorPointsList = new List<Dtos.Get.ConstructorWithTotalFantasyPointScoredGetDto>();

        foreach (var constructor in constructors)
        {
            // Calculate total fantasy points for the constructor in the season and create DTO
            
            // Repository already includes RaceEntries for the season so we can sum directly
            var totalPoints = constructor.RaceEntries
                .Sum(re => re.PointsGained);

            var constructorDto = StatisticDtoMapper.MapToConstructorWithTotalFantasyPointScoredGetDto(constructor, totalPoints);
            constructorPointsList.Add(constructorDto);
        }
        
        return constructorPointsList
            .OrderByDescending(d => d.TotalFantasyPointScored)
            .ToList();
    }
    
    public async Task<List<Dtos.Get.ConstructorWithAveragePointScoredGetDto>> GetTopConstructorsInSeasonByAverageFantasyPointsAsync(int seasonId)
    {
        var constructors = await statisticRepository.GetAllConstructorsIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var constructorPointsList = new List<Dtos.Get.ConstructorWithAveragePointScoredGetDto>();

        foreach (var constructor in constructors)
        {
            // Calculate total fantasy points for the constructor in the season and create DTO
            
            // Repository already includes RaceEntries for the season so we can average directly
            var averageFantasyPointScored = constructor.RaceEntries
                .Average(re => re.PointsGained);

            var constructorDto = StatisticDtoMapper.MapToConstructorWithAverageFantasyPointScoredGetDto(constructor, averageFantasyPointScored);
            constructorPointsList.Add(constructorDto);
        }
        
        return constructorPointsList
            .OrderByDescending(d => d.AverageFantasyPointScored)
            .ToList();
    }

    public async Task<List<Dtos.Get.ConstructorWithSelectionPercentageGetDto>> GetTopConstructorsInSeasonBySelectionPercentageAsync(int seasonId)
    {
        var totalFantasyLineupsInSeason = await statisticRepository.GetTotalNumberOfFantasyLineupForASeasonUntilCurrentDateAsync(seasonId);
        var currentRace = await coreGameplayRepository.GetCurrentRaceAsync();
        if (currentRace != null)
        {
            totalFantasyLineupsInSeason += await statisticRepository.GetTotalNumberOfFantasyLineupForARaceAsync(currentRace.Id);
        }

        var constructors = await statisticRepository.GetAllConstructorsIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var constructorSelectionList = new List<Dtos.Get.ConstructorWithSelectionPercentageGetDto>();

        foreach (var constructor in constructors)
        {
            var totalSelectionsForConstructor = await statisticRepository.GetTotalNumberOfFantasyLineupSelectionForAConstructorInASeasonUntilCurrentDateAsync(seasonId, constructor.Id);
            if (currentRace != null)
            {
                totalSelectionsForConstructor += await statisticRepository.GetTotalNumberOfFantasyLineupSelectionForAConstructorInARaceAsync(currentRace.Id, constructor.Id);
            }
            double selectionPercentage = totalFantasyLineupsInSeason == 0
                ? 0.0
                : (double)totalSelectionsForConstructor / (double)totalFantasyLineupsInSeason;
            
            var constructorDto = StatisticDtoMapper.MapToConstructorWithSelectionPercentageGetDto(constructor, selectionPercentage);
            constructorSelectionList.Add(constructorDto);
        }
        
        return constructorSelectionList
            .OrderByDescending(d => d.SelectionPercentage)
            .ToList();
    }
    
    public async Task<List<Dtos.Get.ConstructorWithPodiumsGetDto>> GetTopConstructorsInSeasonByTotalPodiumsAsync(int seasonId)
    {
        var constructors = await statisticRepository.GetAllConstructorsIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var constructorWithPodiumsList = new List<Dtos.Get.ConstructorWithPodiumsGetDto>();

        foreach (var constructor in constructors)
        {
            // Repository already includes RaceEntries for the season so we can get race podiums directly
            var totalPodiums = constructor.RaceEntries
                .Count(re => re.Position <= 3);

            var constructorDto = StatisticDtoMapper.MapToConstructorWithPodiumsGetDto(constructor, totalPodiums);
            constructorWithPodiumsList.Add(constructorDto);
        }
        
        return constructorWithPodiumsList
            .OrderByDescending(d => d.TotalPodiums)
            .ToList();
    }

    public async Task<List<Dtos.Get.ConstructorWithTop10FinishesGetDto>>
        GetTopConstructorsInSeasonByTotalTop10FinishesAsync(int seasonId)
    {
        var constructors = await statisticRepository.GetAllConstructorsIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var constructorWithPodiumsList = new List<Dtos.Get.ConstructorWithTop10FinishesGetDto>();

        foreach (var constructor in constructors)
        {
            // Repository already includes RaceEntries for the season so we can get race podiums directly
            var totalTop10Finishes = constructor.RaceEntries
                .Count(re => re.Position <= 10);

            var constructorDto = StatisticDtoMapper.MapToConstructorWithTop10FinishesGetDto(constructor, totalTop10Finishes);
            constructorWithPodiumsList.Add(constructorDto);
        }
        
        return constructorWithPodiumsList
            .OrderByDescending(d => d.TotalTop10Finishes)
            .ToList();
    }
}