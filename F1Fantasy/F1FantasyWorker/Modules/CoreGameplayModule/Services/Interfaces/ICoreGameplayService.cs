using F1FantasyWorker.Core.Common;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Services.Interfaces;

public interface ICoreGameplayService
{
    void CalculatePointsForAllUsersInLastestFinishedRaceAsync();
    
}