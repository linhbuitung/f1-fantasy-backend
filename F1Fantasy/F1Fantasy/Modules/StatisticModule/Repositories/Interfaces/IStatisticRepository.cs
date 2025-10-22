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
    
    Task<List<Driver>> GetAllDriversIncludeRaceEntriesBySeasonIdAsync(int seasonId);
    
    // Get total number of fantasy lineup created in a season from the first race to the current race
    Task<int> GetTotalNumberOfFantasyLineupForASeasonUntilCurrentDateAsync(int seasonId);
    
    Task<int> GetTotalNumberOfFantasyLineupForARaceAsync(int raceId);

    Task<int> GetTotalNumberOfFantasyLineupSelectionForADriverInASeasonUntilCurrentDateAsync(int seasonId, int driverId);
    
    Task<int> GetTotalNumberOfFantasyLineupSelectionForADriverInARaceAsync(int raceId, int driverId);

}