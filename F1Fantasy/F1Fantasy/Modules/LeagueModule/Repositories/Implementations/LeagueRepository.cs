using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.LeagueModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.LeagueModule.Repositories.Implementations;

public class LeagueRepository(WooF1Context context) : ILeagueRepository
{
    public async Task<League> AddLeagueAsync(League league)
    {
        context.Leagues.Add(league);
        await context.SaveChangesAsync();
        return league;
    }

    public async Task<League?> GetTrackedLeagueByLeagueIdAndOwnerIdAsync(int leagueId, int ownerId)
    {
        return await context.Leagues
            .AsTracking()
            .FirstOrDefaultAsync(l => l.Id == leagueId && l.OwnerId == ownerId);
    }

    public async Task<League?> GetLeagueByNameAsync(string leagueName)
    {
        return await context.Leagues
            .FirstOrDefaultAsync(l => l.Name.Equals(leagueName));
    }
    public async Task<UserLeague> AddUserLeagueAsync(UserLeague userLeague)
    {
        context.UserLeagues.Add(userLeague);
        await context.SaveChangesAsync();
        return userLeague;
    }

    public async Task<League?> GetLeagueByIdIncludesOwnerAndPlayersAsync(int leagueId)
    {
        return await context.Leagues
            .Include(l => l.User)
            .ThenInclude(o => o.Country)
            .Include(l => l.UserLeagues)
            .FirstOrDefaultAsync(l => l.Id == leagueId);
    }
    
    public async Task<List<League>> GetAllLeaguesByOwnerIdAsync(int ownerId)
    {
        return await context.Leagues
            .Where(l => l.OwnerId == ownerId)
            .ToListAsync();
    }
    
    public async Task<List<League>> GetAllLeaguesByJoinedPlayerIdAsync(int playerId)
    {
        return await context.UserLeagues
            .Where(ul => ul.UserId == playerId && ul.IsAccepted)
            .Select(ul => ul.League)
            .ToListAsync();
    }

    public async Task DeleteLeagueByIdAsync(int leagueId)
    {
        // Delete related UserLeagues first due to foreign key constraints
        var userLeagues = context.UserLeagues.Where(ul => ul.LeagueId == leagueId);
        context.UserLeagues.RemoveRange(userLeagues);
        
        var league = await context.Leagues.FindAsync(leagueId);
        if (league != null)
        {
            context.Leagues.Remove(league);
            await context.SaveChangesAsync();
        }
    }

    public async Task<UserLeague?> GetUserLeagueByIdAsync(int leagueId, int playerId)
    {
        return await context.UserLeagues
            .Include(l => l.User)
            .FirstOrDefaultAsync(ul => ul.LeagueId == leagueId && ul.UserId == playerId);
    }

    public async Task<UserLeague> UpdateUserLeagueAsync(UserLeague userLeague)
    {
        var existingUserLeague = await context.UserLeagues
            .Include(l => l.User)
            .AsTracking()
            .FirstOrDefaultAsync(ul => ul.LeagueId == userLeague.LeagueId && ul.UserId == userLeague.UserId);
        
        if (existingUserLeague != null)
        {
            existingUserLeague.IsAccepted = userLeague.IsAccepted;
            await context.SaveChangesAsync();
            return existingUserLeague;
        }

        throw new InvalidOperationException("UserLeague not found.");
    }

    public async Task DeleteUserLeagueByIdAsync(int leagueId, int playerId)
    {
        var existingUserLeague = await context.UserLeagues
            .AsTracking()
            .FirstOrDefaultAsync(ul => ul.LeagueId == leagueId && ul.UserId == playerId);
        
        if (existingUserLeague != null)
        {
            context.UserLeagues.Remove(existingUserLeague);
            await context.SaveChangesAsync();
            return;
        }

        throw new InvalidOperationException("UserLeague not found.");
    }
    public async Task<List<UserLeague>> GetAllWaitingJoinRequestsByLeagueIdAsync(int leagueId)
    {
        return await context.UserLeagues
            .Include(ul => ul.User)
            .Where(ul => ul.LeagueId == leagueId && !ul.IsAccepted).ToListAsync();
    }
    
    public async Task<List<UserLeague>> GetAllUserLeaguesByLeagueIdAndAcceptStatusAsync(int leagueId, bool isAccepted, int? currentSeasonId)
    {
        if(isAccepted && currentSeasonId != null)
        {
            return await context.UserLeagues
                .Include(ul => ul.User)
                .ThenInclude(u => u.FantasyLineups)
                .ThenInclude(fl => fl.Race)
                .Where(ul => ul.LeagueId == leagueId && ul.IsAccepted == isAccepted)
                .AsNoTracking()
                .ToListAsync();
        }
        return await context.UserLeagues
            .Include(ul => ul.User)
            .Where(ul => ul.LeagueId == leagueId && ul.IsAccepted == isAccepted)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<(List<League> Leagues, int TotalCount)> SearchLeaguesAsync(string query, int skip, int take, LeagueType? leagueType)
    {
        System.Linq.IQueryable<League> leaguesQuery;
        if (leagueType == null)
        {
            leaguesQuery = context.Leagues
                .Where(l =>
                    EF.Functions.ILike(l.Name, $"%{query}%") ||
                     EF.Functions.ILike(l.Description, $"%{query}%"))
                .Include(l => l.User);
        }
        else
        {
            leaguesQuery = context.Leagues
                .Where(l =>
                    (EF.Functions.ILike(l.Name, $"%{query}%") ||
                     EF.Functions.ILike(l.Description, $"%{query}%")) &&
                    l.Type == leagueType)
                .Include(l => l.User);
        }


        var totalCount = await leaguesQuery.CountAsync();
        var leagues = await leaguesQuery
            .OrderBy(l => l.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (leagues, totalCount);
    }
    
    public async Task<(List<League> Leagues, int TotalCount)> GetLeaguesAsync(int skip, int take, LeagueType? leagueType)
    {
        List<League> leagues;
        var totalCount = 0;
        if (leagueType == null)
        {
            leagues = await context.Leagues
                .Include(l => l.User)
                .OrderBy(l => l.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            totalCount = await context.Leagues
                .CountAsync();
        }
        else 
        {
            leagues  = await context.Leagues
                .Where(l => l.Type == leagueType)
                .Include(l => l.User)
                .OrderBy(l => l.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            totalCount  = await context.Leagues
                .Where(l => l.Type == leagueType)
                .CountAsync();
        }

        return ( leagues, totalCount);
    }
}