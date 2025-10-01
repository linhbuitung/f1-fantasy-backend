using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.LeagueModule.Repositories.Interfaces;

public interface ILeagueRepository
{
    Task<League> AddLeagueAsync(League league);
    
    Task<League?> GetTrackedLeagueByLeagueIdAndOwnerIdAsync(int leagueId, int ownerId);
    
    Task<League?> GetLeagueByNameAsync(string leagueName);
    Task<UserLeague> AddUserLeagueAsync(UserLeague userLeague);
    
    Task<League?> GetLeagueByIdIncludesOwnerAndPlayersAsync(int leagueId);

    Task<List<League>> GetAllLeaguesByOwnerIdAsync(int ownerId);

    Task<List<League>> GetAllLeaguesByJoinedPlayerIdAsync(int playerId);
    
    Task<List<UserLeague>> GetAllWaitingJoinRequestsByLeagueIdAsync(int leagueId);
    
    Task<UserLeague?> GetUserLeagueByIdAsync(int leagueId, int playerId);
    
    Task<UserLeague> UpdateUserLeagueAsync(UserLeague userLeague);
    
    Task DeleteUserLeagueByIdAsync(int leagueId, int playerId);
    Task DeleteLeagueByIdAsync(int leagueId);
    
    Task<List<UserLeague>> GetAllUserLeaguesByLeagueIdAndAcceptStatusAsync(int leagueId, bool isAccepted);
    
    public Task<(List<League> Leagues, int TotalCount)> SearchLeaguesViaFullTextSearchAsync(string query, int skip, int take);
}