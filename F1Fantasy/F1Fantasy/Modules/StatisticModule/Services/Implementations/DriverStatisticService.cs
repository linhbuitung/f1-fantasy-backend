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

            var driverDto = StatisticDtoMapper.MapToDriverWithTotalFantasyPointScoredGetDto(driver, totalPoints);
            driverPointsList.Add(driverDto);
        }
        
        return driverPointsList
            .OrderByDescending(d => d.TotalFantasyPointScored)
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

            var driverDto = StatisticDtoMapper.MapToDriverWithAverageFantasyPointScoredGetDto(driver, averageFantasyPointScored);
            driverPointsList.Add(driverDto);
        }
        
        return driverPointsList
            .OrderByDescending(d => d.AverageFantasyPointScored)
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
            .OrderByDescending(d => d.SelectionPercentage)
            .ToList();
    }

    public async Task<List<Dtos.Get.DriverWithRaceWinsGetDto>> GetTopDriversInSeasonByTotalRacesWinsAsync(int seasonId)
    {
        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverWithRaceWinsList = new List<Dtos.Get.DriverWithRaceWinsGetDto>();

        foreach (var driver in drivers)
        {
            // Repository already includes RaceEntries for the season so we can get race wins directly
            var totalRaceWins = driver.RaceEntries
                .Count(re => re.Position == 1);

            var driverDto = StatisticDtoMapper.MapToDriverWithRaceWinsGetDto(driver, totalRaceWins);
            driverWithRaceWinsList.Add(driverDto);
        }
        
        return driverWithRaceWinsList
            .OrderByDescending(d => d.TotalRacesWin)
            .ToList();
    }

    public async Task<List<Dtos.Get.DriverWithPodiumsGetDto>> GetTopDriversInSeasonByTotalPodiumsAsync(int seasonId)
    {
        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverWithPodiumsList = new List<Dtos.Get.DriverWithPodiumsGetDto>();

        foreach (var driver in drivers)
        {
            // Repository already includes RaceEntries for the season so we can get race podiums directly
            var totalPodiums = driver.RaceEntries
                .Count(re => re.Position <= 3);

            var driverDto = StatisticDtoMapper.MapToDriverWithPodiumsGetDto(driver, totalPodiums);
            driverWithPodiumsList.Add(driverDto);
        }
        
        return driverWithPodiumsList
            .OrderByDescending(d => d.TotalPodiums)
            .ToList();
    }

    public async Task<List<Dtos.Get.DriverWithTop10FinishesGetDto>>
        GetTopDriversInSeasonByTotalTop10FinishesAsync(int seasonId)
    {
        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverWithTop10sList = new List<Dtos.Get.DriverWithTop10FinishesGetDto>();

        foreach (var driver in drivers)
        {
            // Repository already includes RaceEntries for the season so we can get top 10s directly
            var totalTop10Finishes = driver.RaceEntries
                .Count(re => re.Position <= 10);

            var driverDto = StatisticDtoMapper.MapToDriverWithTop10FinishesGetDto(driver, totalTop10Finishes);
            driverWithTop10sList.Add(driverDto);
        }
        
        return driverWithTop10sList
            .OrderByDescending(d => d.TotalTop10Finishes)
            .ToList();
    }

    public async Task<List<Dtos.Get.DriverWithFastestLapsGetDto>> GetTopDriversInSeasonByTotalFastestLapsAsync(
        int seasonId)
    {
        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverWithFastestLapsList = new List<Dtos.Get.DriverWithFastestLapsGetDto>();

        foreach (var driver in drivers)
        {
            // Repository already includes RaceEntries for the season so we can get top 10s directly
            var totalFastestLap = driver.RaceEntries
                .Count(re => re.FastestLap == 1);

            var driverDto = StatisticDtoMapper.MapToDriverWithFastestLapsGetDto(driver, totalFastestLap);
            driverWithFastestLapsList.Add(driverDto);
        }
        
        return driverWithFastestLapsList
            .OrderByDescending(d => d.TotalFastestLaps)
            .ToList();
    }

    public async Task<List<Dtos.Get.DriverWithDnfsGetDto>> GetTopDriversInSeasonByTotalDnfsAsync(int seasonId)
    {
        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverWithDnfsList = new List<Dtos.Get.DriverWithDnfsGetDto>();

        foreach (var driver in drivers)
        {
            // Repository already includes RaceEntries for the season so we can get top 10s directly
            var totalDnfs = driver.RaceEntries
                .Count(re => !re.Finished);

            var driverDto = StatisticDtoMapper.MapToDriverWithDnfsGetDto(driver, totalDnfs);
            driverWithDnfsList.Add(driverDto);
        }
        
        return driverWithDnfsList
            .OrderByDescending(d => d.TotalDnfs)
            .ToList();
    }
}