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
            .Include(l => l.UserLeagues)
            .ThenInclude(ul => ul.User)
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
    
    public async Task<List<UserLeague>> GetAllUserLeaguesByLeagueIdAndAcceptStatusAsync(int leagueId, bool isAccepted)
    {
        return await context.UserLeagues
            .Include(ul => ul.User)
            .Where(ul => ul.LeagueId == leagueId && ul.IsAccepted == isAccepted)
            .ToListAsync();
    }
    public async Task<(List<League> Leagues, int TotalCount)> SearchLeaguesViaFullTextSearchAsync(string query, int skip, int take)
    {
        var leaguesQuery = context.Leagues
            .Where(l =>
                EF.Functions.ToTsVector("english", l.Name + " " + l.Description)
                    .Matches(EF.Functions.PlainToTsQuery("english", query)))
            .Include(l => l.User);

        var totalCount = await leaguesQuery.CountAsync();
        var leagues = await leaguesQuery
            .OrderBy(l => l.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (leagues, totalCount);
    }
    
}