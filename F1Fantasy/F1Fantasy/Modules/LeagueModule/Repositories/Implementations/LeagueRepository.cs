using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.LeagueModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.LeagueModule.Repositories.Implementations;

public class LeagueRepository : ILeagueRepository
{
    private readonly WooF1Context _context;

    public LeagueRepository(WooF1Context context)
    {
        _context = context;
    }
    
    public async Task<League> AddLeagueAsync(League league)
    {
        _context.Leagues.Add(league);
        await _context.SaveChangesAsync();
        return league;
    }

    public async Task<UserLeague> AddUserLeagueAsync(UserLeague userLeague)
    {
        _context.UserLeagues.Add(userLeague);
        await _context.SaveChangesAsync();
        return userLeague;
    }

    public async Task<League?> GetLeagueByIdIncludesOwnerAndPlayersAsync(int leagueId)
    {
        return await _context.Leagues
            .Include(l => l.UserLeagues)
            .ThenInclude(ul => ul.User)
            .Include(l => l.User).FirstOrDefaultAsync(l => l.Id == leagueId);
    }
    
    public async Task<List<League>> GetAllLeaguesByOwnerIdAsync(int ownerId)
    {
        return await _context.Leagues
            .Where(l => l.OwnerId == ownerId)
            .ToListAsync();
    }
    
    public async Task<List<League>> GetAllLeaguesByJoinedPlayerIdAsync(int ownerId)
    {
        return await _context.UserLeagues
            .Where(ul => ul.UserId == ownerId)
            .Select(ul => ul.League)
            .ToListAsync();
    }

    public async Task DeleteLeagueByIdAsync(int leagueId)
    {
        // Delete related UserLeagues first due to foreign key constraints
        var userLeagues = _context.UserLeagues.Where(ul => ul.LeagueId == leagueId);
        _context.UserLeagues.RemoveRange(userLeagues);
        
        var league = await _context.Leagues.FindAsync(leagueId);
        if (league != null)
        {
            _context.Leagues.Remove(league);
            await _context.SaveChangesAsync();
        }
    }
}