using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;

public interface IStatisticRepository
{
    Task<int> GetHighestScoreBySeasonIdAsync(int seasonId);
    
    Task<double> GetAverageScoreBySeasonIdAsync(int seasonId);
    
    Task<string> GetMostPickedDriverAsync(int seasonId);
    
    Task<int> GetTotalTransfersBySeasonIdAsync(int seasonId);
    
    Task<FantasyLineup?> GetBestFantasyLineupOfAnUserBySeasonIdAsync(int userId, int seasonId);
    
    Task<int> GetTotalPointOfAnUserBySeasonIdAsync(int userId, int seasonId);
    
    Task<int> GetTotalTransfersOfAnUserBySeasonIdAsync(int userId, int seasonId);
    
    Task<int> GetOverallRankOfAnUserBySeasonIdAsync(int userId, int seasonId);

    Task<List<RaceEntry>> GetTopDriverRaceEntriesInARaceByRaceIdAsync(int raceId, int topN);
    
    Task<List<RaceEntry>> GetTopConstructorRaceEntriesInARaceByRaceIdAsync(int raceId, int topN);
    
    Task<List<RaceEntry>> GetAllRaceEntriesByRaceIdAsync(int raceId);

}