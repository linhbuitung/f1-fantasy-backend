namespace F1Fantasy.Modules.StatisticModule.Services.Interfaces;

public interface IConstructorStatisticService
{
    Task<List<Dtos.Get.ConstructorWithTotalFantasyPointScoredGetDto>> GetTopConstructorsInSeasonByTotalFantasyPointsAsync(int seasonId);
    
    Task<List<Dtos.Get.ConstructorWithAveragePointScoredGetDto>> GetTopConstructorsInSeasonByAverageFantasyPointsAsync(int seasonId);

    Task<List<Dtos.Get.ConstructorWithSelectionPercentageGetDto>> GetTopConstructorsInSeasonBySelectionPercentageAsync(int seasonId);
    
    Task<List<Dtos.Get.ConstructorWithPodiumsGetDto>> GetTopConstructorsInSeasonByTotalPodiumsAsync(int seasonId);
    
    Task<List<Dtos.Get.ConstructorWithTop10FinishesGetDto>> GetTopConstructorsInSeasonByTotalTop10FinishesAsync(int seasonId);
}