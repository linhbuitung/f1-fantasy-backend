using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.CoreGameplayModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Repositories.Implementations;

public class CoreGameplayRepository(WooF1Context context) : ICoreGameplayRepository
{
    public async Task<IEnumerable<RaceEntry>> GetRaceEntriesByRaceDateAsync(DateOnly date)
    {
        return await context.RaceEntries.Where(re => re.Race.RaceDate == date).ToListAsync();
    }

    public async Task<Race?> GetLatestFinishedRaceInCurrentSeasonWithResultAsync()
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        return await context.Races
            .Where(r => r.DeadlineDate < currentDate && r.RaceDate > currentDate)
            .OrderByDescending(r => r.DeadlineDate)
            .Include(r => r.RaceEntries)
            .AsTracking()
            .FirstOrDefaultAsync();
    }

    public async Task AddFantasyLineupDriverAsync(FantasyLineup fantasyLineup, Driver driver)
    {
        fantasyLineup.DriversNavigation.Add(driver);
        await context.SaveChangesAsync();
    }
}