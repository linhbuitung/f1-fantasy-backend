using F1FantasyWorker.Core.Common;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Repositories.Interfaces;

public interface ICoreGameplayRepository
{
    Task<IEnumerable<RaceEntry>> GetRaceEntriesByRaceDateAsync(DateOnly date);
    
    Task<Race?> GetLatestFinishedRaceInCurrentSeasonWithResultAsync();

    Task AddFantasyLineupDriverAsync(FantasyLineup fantasyLineup, Driver driver);
}