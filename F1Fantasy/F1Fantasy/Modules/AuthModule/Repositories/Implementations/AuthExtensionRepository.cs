using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AuthModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.AuthModule.Repositories.Implementations;

public class AuthExtensionRepository(WooF1Context context) : IAuthExtensionRepository
{
    public async Task<List<int>> GetAllRaceIdsByYearAsync(int year)
    {
        return await context.Races.AsNoTracking().Where(r => r.Season.Year == year).Select(r => r.Id).ToListAsync();
    }
    
    public async Task<FantasyLineup?> GetFantasyLineupByUserIdAndRaceId(int userId, int raceId)
    {
        return await context.FantasyLineups.AsNoTracking().FirstOrDefaultAsync(f => f.UserId.Equals(userId) && f.RaceId.Equals(raceId));
    }
    
    public async Task<FantasyLineup> AddFantasyLineupAsync(FantasyLineup fantasyLineup)
    {
        context.FantasyLineups.Add(fantasyLineup);
        await context.SaveChangesAsync();
        return fantasyLineup;
    }
}