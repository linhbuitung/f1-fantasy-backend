using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.CoreGameplayModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Repositories.Implementations;

public class CoreGameplayRepository : ICoreGameplayRepository
{
    private readonly WooF1Context _context;

    public CoreGameplayRepository(WooF1Context context)
    {
        _context = context;
    }
    public async Task<IEnumerable<RaceEntry>> GetRaceEntriesByRaceDateAsync(DateOnly date)
    {
        return await _context.RaceEntries.Where(re => re.Race.RaceDate == date).ToListAsync();
    }

    public async Task<Race?> GetLatestFinishedRaceInCurrentSeasonWithResultAsync()
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        return await _context.Races
            .Where(r => r.DeadlineDate < currentDate && r.RaceDate > currentDate)
            .OrderByDescending(r => r.DeadlineDate)
            .Include(r => r.RaceEntries)
            .AsTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<FantasyLineupDriver> AddFantasyLineupDriverAsync(FantasyLineupDriver fantasyLineupDriver)
    {
        await _context.AddAsync(fantasyLineupDriver);
        await _context.SaveChangesAsync();
        return fantasyLineupDriver;
    }
}