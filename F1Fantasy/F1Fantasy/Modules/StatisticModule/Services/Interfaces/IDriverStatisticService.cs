namespace F1Fantasy.Modules.StatisticModule.Services.Interfaces;

public interface IDriverStatisticService
{
    Task<List<Dtos.Get.DriverWithTotalFantasyPointScoredGetDto>> GetTopDriversInSeasonByTotalFantasyPointsAsync(int seasonId);
    
    Task<List<Dtos.Get.DriverWithAveragePointScoredGetDto>> GetTopDriversInSeasonByAverageFantasyPointsAsync(int seasonId);

    Task<List<Dtos.Get.DriverWithSelectionPercentageGetDto>> GetTopDriversInSeasonBySelectionPercentageAsync(int seasonId);
    
    Task<List<Dtos.Get.DriverWithRaceWinsGetDto>> GetTopDriversInSeasonByTotalRacesWinsAsync(int seasonId);

    Task<List<Dtos.Get.DriverWithPodiumsGetDto>> GetTopDriversInSeasonByTotalPodiumsAsync(int seasonId);
    Task<List<Dtos.Get.DriverWithTop10FinishesGetDto>> GetTopDriversInSeasonByTotalTop10FinishesAsync(int seasonId);
    Task<List<Dtos.Get.DriverWithFastestLapsGetDto>> GetTopDriversInSeasonByTotalFastestLapsAsync(int seasonId);
    Task<List<Dtos.Get.DriverWithDnfsGetDto>> GetTopDriversInSeasonByTotalDnfsAsync(int seasonId);
}