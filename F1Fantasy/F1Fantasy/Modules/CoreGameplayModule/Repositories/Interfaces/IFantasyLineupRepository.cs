using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.CoreGameplayModule.Repositories.Interfaces;

public interface IFantasyLineupRepository
{
    Task<FantasyLineup?> GetFantasyLineupByIdAsync(int fantasyLineupId);

    Task<FantasyLineup?> GetFantasyLineupByIdAsTrackingAsync(int fantasyLineupId);
    
    Task<FantasyLineup?> GetFantasyLineupByUserIdAndRaceIdAsync(int userId, int raceId);
    Task<FantasyLineup?> GetCurrentFantasyLineupByUserIdAsync(int userId);
    
    Task<FantasyLineup> UpdateFantasyLineupAsync(
        List<int> driverIds, 
        List<int> constructorIds, 
        FantasyLineup trackedFantasyLineup,
        int? captainDriverId,
        int maxDrivers,
        int maxConstructors);
    
    Task<FantasyLineup> UpdateFantasyLineupAsync(
        List<int> driverIds, 
        List<int> constructorIds, 
        List<int> powerupIds, 
        FantasyLineup trackedFantasyLineup,
        int? captainDriverId,
        int maxDrivers,
        int maxConstructors);

    Task UpdateAllFutureFantasyLineupAsync(
        FantasyLineup trackedFantasyLineup) ;
    Task ResetFantasyLineupsBySeasonAsync(Season season);
}