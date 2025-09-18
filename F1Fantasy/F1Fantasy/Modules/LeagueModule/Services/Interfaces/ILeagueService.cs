using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.LeagueModule.Services.Interfaces;

public interface ILeagueService
{
    Task<Dtos.Get.LeagueDto> AddLeagueAsync(Dtos.Create.LeagueDto leagueCreateDto, LeagueType leagueType);
    
    Task<Dtos.Get.LeagueDto> GetLeagueByIdAsync(int leagueId, int pageNum, int pageSize);
    
    Task DeleteLeagueByIdAsync(int leagueId);
    
    Task JoinLeagueAsync(int leagueId, int playerId);

    Task<List<Dtos.Get.UserLeagueDto>> GetAllWaitingJoinRequestsAsync(int leagueId);

    Task<Dtos.Get.UserLeagueDto> HandleJoinRequestAsync(Dtos.Update.UserLeagueDto userLeagueDto);
    
    Task<Dtos.Get.UserLeagueDto> GetUserLeagueByIdAsync(int leagueId, int playerId);
    // This method is for the owner to accept or reject a join request
    Task LeaveLeagueAsync(int leagueId, int playerId);
}