using F1Fantasy.Modules.AdminModule.Repositories.Interfaces;
using F1Fantasy.Modules.CoreGameplayModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Dtos.Mapper;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;

namespace F1Fantasy.Modules.StatisticModule.Services.Implementations;

public class DriverStatisticService(IStatisticRepository statisticRepository, ICoreGameplayRepository coreGameplayRepository) : IDriverStatisticService
{
    public async Task<List<Dtos.Get.DriverWithTotalFantasyPointScoredGetDto>> GetTopDriversInSeasonByTotalFantasyPointsAsync(int seasonId)
    {
        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverPointsList = new List<Dtos.Get.DriverWithTotalFantasyPointScoredGetDto>();

        foreach (var driver in drivers)
        {
            // Calculate total fantasy points for the driver in the season and create DTO
            
            // Repository already includes RaceEntries for the season so we can sum directly
            var totalPoints = driver.RaceEntries
                .Sum(re => re.PointsGained);

            var driverDto = StatisticDtoMapper.MapToDriverWithTotalFantasyPointScoredDto(driver, totalPoints);
            driverPointsList.Add(driverDto);
        }
        
        return driverPointsList
            .OrderByDescending(dp => dp.TotalFantasyPointScored)
            .ToList();
    }

    public async Task<List<Dtos.Get.DriverWithAveragePointScoredGetDto>>
        GetTopDriversInSeasonByAverageFantasyPointsAsync(int seasonId)
    {
        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverPointsList = new List<Dtos.Get.DriverWithAveragePointScoredGetDto>();

        foreach (var driver in drivers)
        {
            // Calculate total fantasy points for the driver in the season and create DTO
            
            // Repository already includes RaceEntries for the season so we can average directly
            var averageFantasyPointScored = driver.RaceEntries
                .Average(re => re.PointsGained);

            var driverDto = StatisticDtoMapper.MapToDriverWithAverageFantasyPointScoredDto(driver, averageFantasyPointScored);
            driverPointsList.Add(driverDto);
        }
        
        return driverPointsList
            .OrderByDescending(dp => dp.AverageFantasyPointScored)
            .ToList();
    }

    public async Task<List<Dtos.Get.DriverWithSelectionPercentageGetDto>>
        GetTopDriversInSeasonBySelectionPercentageAsync(int seasonId)
    {
        var totalFantasyLineupsInSeason = await statisticRepository.GetTotalNumberOfFantasyLineupForASeasonUntilCurrentDateAsync(seasonId);
        var currentRace = await coreGameplayRepository.GetCurrentRaceAsync();
        if (currentRace != null)
        {
            totalFantasyLineupsInSeason += await statisticRepository.GetTotalNumberOfFantasyLineupForARaceAsync(currentRace.Id);
        }

        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverSelectionList = new List<Dtos.Get.DriverWithSelectionPercentageGetDto>();

        foreach (var driver in drivers)
        {
            var totalSelectionsForDriver = await statisticRepository.GetTotalNumberOfFantasyLineupSelectionForADriverInASeasonUntilCurrentDateAsync(seasonId, driver.Id);
            if (currentRace != null)
            {
                totalSelectionsForDriver += await statisticRepository.GetTotalNumberOfFantasyLineupSelectionForADriverInARaceAsync(currentRace.Id, driver.Id);
            }
            double selectionPercentage = totalFantasyLineupsInSeason == 0
                ? 0.0
                : (double)totalSelectionsForDriver / (double)totalFantasyLineupsInSeason;
            
            var driverDto = StatisticDtoMapper.MapToDriverWithSelectionPercentageDto(driver, selectionPercentage);
            driverSelectionList.Add(driverDto);
        }
        
        return driverSelectionList
            .OrderByDescending(dp => dp.SelectionPercentage)
            .ToList();
    }
}