namespace F1Fantasy.Modules.StatisticModule.Services.Interfaces;

public interface IUserStatisticService
{
    Task<List<Dtos.Get.UserWithTotalFantasyPointScoredGetDto>> GetTopUsersInSeasonByTotalFantasyPointsAsync(int seasonId);
    
    Task<List<Dtos.Get.UserWithAveragePointScoredGetDto>> GetTopUsersInSeasonByAverageFantasyPointsAsync(int seasonId);

}