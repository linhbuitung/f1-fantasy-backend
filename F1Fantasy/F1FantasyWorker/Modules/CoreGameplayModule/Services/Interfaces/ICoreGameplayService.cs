using F1FantasyWorker.Core.Common;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Services.Interfaces;

public interface ICoreGameplayService
{
    Task CalculatePointsForAllUsersInLastestFinishedRaceAsync();
    
    Task MigrateFantasyLineupsToRaceAsync(int raceToBeMigratedId);
}