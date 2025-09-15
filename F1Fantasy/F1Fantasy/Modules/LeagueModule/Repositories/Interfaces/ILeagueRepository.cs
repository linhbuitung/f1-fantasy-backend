using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.LeagueModule.Repositories.Interfaces;

public interface ILeagueRepository
{
    Task<League> AddLeagueAsync(League league);
    
    Task<UserLeague> AddUserLeagueAsync(UserLeague userLeague);
    
    Task<League?> GetLeagueByIdIncludesOwnerAndPlayersAsync(int leagueId);

    Task<List<League>> GetAllLeaguesByOwnerIdAsync(int ownerId);

    Task<List<League>> GetAllLeaguesByJoinedPlayerIdAsync(int ownerId);
    
    Task DeleteLeagueByIdAsync(int leagueId);
}