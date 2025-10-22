namespace F1Fantasy.Modules.StatisticModule.Services.Interfaces;

public interface IDriverStatisticService
{
    Task<List<Dtos.Get.DriverWithTotalFantasyPointScoredGetDto>> GetTopDriversInSeasonByTotalFantasyPointsAsync(int seasonId);
    
    Task<List<Dtos.Get.DriverWithAveragePointScoredGetDto>> GetTopDriversInSeasonByAverageFantasyPointsAsync(int seasonId);

    Task<List<Dtos.Get.DriverWithSelectionPercentageGetDto>> GetTopDriversInSeasonBySelectionPercentageAsync(int seasonId);
}