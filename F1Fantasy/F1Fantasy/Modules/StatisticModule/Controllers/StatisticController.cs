using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StatisticModule.Controllers;

[ApiController]
[Route("statistic")]
public class StatisticController(IStatisticService statisticService, IAdminService adminService, ICoreGameplayService coreGameplayService) : ControllerBase
{
    [HttpGet("general/current-season")]
    public async Task<IActionResult> GetGeneralStatisticForCurrentActiveSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var statisticDto = await statisticService.GetGeneralStatisticBySeasonId(activeSeason.Id);

        return Ok(statisticDto);
    }
    
    [Authorize]
    [HttpGet("general/current-season/user/{userId}")]
    public async Task<IActionResult> GetGeneralStatisticForCurrentActiveSeasonByUserId(int userId)
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var statisticDto = await statisticService.GetUserGeneralStatisticByUserIdAndSeasonIdAsync(userId, activeSeason.Id);

        return Ok(statisticDto);
    }
    
        
    [HttpGet("general/team-of-the-race/latest")]
    public async Task<IActionResult> GetTeamOfTheRaceForCurrentActiveSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        var latestRace = await coreGameplayService.GetLatestFinishedRaceAsync();
        
        var teamOfTheRaceDto = await statisticService.GetTeamOfTheRaceByRaceIdAsync(latestRace);

        return Ok(teamOfTheRaceDto);
    }
}