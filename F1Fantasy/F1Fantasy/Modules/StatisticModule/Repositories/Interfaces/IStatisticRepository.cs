namespace F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;

public interface IStatisticRepository
{
    Task<int> GetHighestScoreBySeasonIdAsync(int seasonId);
    
    Task<double> GetAverageScoreBySeasonIdAsync(int seasonId);
    
    Task<string> GetMostPickedDriverAsync(int seasonId);
    
    Task<int> GetTotalTransfersBySeasonIdAsync(int seasonId);
}