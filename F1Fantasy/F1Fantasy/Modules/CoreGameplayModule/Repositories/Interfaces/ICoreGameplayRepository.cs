using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.CoreGameplayModule.Repositories.Interfaces;

public interface ICoreGameplayRepository
{
    Task<PickableItem?>  GetPickableItemsAsync();
    
    Task<List<Powerup>> GetAllPowerupsAsync();
    
    Task<List<Powerup>> GetAvailablePowerupsByUserIdAndSeasonIdAsync(int userId, int seasonId);
    
    // Similar to GetAvailablePowerupsByUserIdAndSeasonIdAsync but checks powerups used in the fantasy lineup identified by fantasyLineupId
    Task<List<Powerup>> GetAvailablePowerupsByFantasyLineupIdAsync(int fantasyLineupId);

    Task<List<int>> GetNonExistentDriverIdsAsync(List<int> driverIds);
    
    Task<List<int>> GetNonExistentConstructorIdsAsync(List<int> constructorIds);
    
    Task<List<int>> GetNonExistentPowerupIdsAsync(List<int> powerupIds);
    Task<Race?> GetLatestFinishedRaceAsync();
    
    Task<Race?> GetLatestFinishedRaceResultAsync();
    
    Task<Race?> GetLatestRaceAsync();
    
    Task<Race?> GetCurrentRaceAsync();
    
    Task<List<Powerup>> GetAllUsedPowerupsOfAnUserInSeasonBeforeCurrentRaceAsync(int userId, Race currentRace);
    
    Task<PowerupFantasyLineup> AddPowerupToFantasyLineupAsync(FantasyLineup fantasyLineup, Powerup powerup);
    
    Task RemovePowerupFromFantasyLineupAsync(FantasyLineup fantasyLineup, Powerup powerup);

}