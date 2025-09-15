using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.LeagueModule.Services.Interfaces;

public interface ILeagueService
{
    Task<Dtos.Get.LeagueDto> AddLeagueAsync(Dtos.Create.LeagueDto leagueCreateDto, LeagueType leagueType);
    
    Task<Dtos.Get.LeagueDto> GetLeagueByIdAsync(int leagueId, int pageNum, int pageSize);
    
    Task DeleteLeagueByIdAsync(int leagueId);
    
    Task JoinLeagueAsync(int leagueId, int playerId);
}