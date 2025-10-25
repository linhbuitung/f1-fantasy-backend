using F1Fantasy.Core.Common;
using F1Fantasy.Shared.Dtos;

namespace F1Fantasy.Modules.LeagueModule.Services.Interfaces;

public interface ILeagueService
{
    Task<Dtos.Get.LeagueDto> AddLeagueAsync(Dtos.Create.LeagueDto leagueCreateDto, LeagueType leagueType);
    
    Task<Dtos.Get.LeagueDto> GetLeagueByIdAsync(int leagueId, int pageNum, int pageSize);
    
    Task<List<Dtos.Get.LeagueDto>> GetJoinedLeaguesByUserIdAsync(int userId);
    Task<List<Dtos.Get.LeagueDto>> GetOwnedLeaguesByUserIdAsync(int userId);
    Task DeleteLeagueByIdAsync(int leagueId);
    
    Task JoinLeagueAsync(int leagueId, int playerId);

    Task<List<Dtos.Get.UserLeagueDto>> GetAllWaitingJoinRequestsAsync(int leagueId);

    Task<Dtos.Get.UserLeagueDto?> HandleJoinRequestAsync(Dtos.Update.UserLeagueDto userLeagueDto);
    
    Task<Dtos.Get.UserLeagueDto> GetUserLeagueByIdAsync(int leagueId, int playerId);
    
    Task<List<Dtos.Get.UserLeagueDto>> GetUnAcceptedUserLeagueByLeagueIdAsync(int leagueId);

    // This method is for the owner to accept or reject a join request
    Task RemovePlayerFromLeagueAsync(int leagueId, int playerId);
    
    Task<PagedResult<Dtos.Get.LeagueDto>> SearchLeaguesAsync(string query, int pageNum, int pageSize);
    
    Task<PagedResult<Dtos.Get.LeagueDto>> GetLeaguesAsync(int pageNum, int pageSize);

    Task<PagedResult<Dtos.Get.LeagueDto>> SearchPublicLeaguesAsync(string query, int pageNum, int pageSize);

    Task<PagedResult<Dtos.Get.LeagueDto>> GetPublicLeaguesAsync(int pageNum, int pageSize);

    Task<Dtos.Get.LeagueDto> UpdateLeagueAsync(Dtos.Update.LeagueDto leagueUpdateDto);
}