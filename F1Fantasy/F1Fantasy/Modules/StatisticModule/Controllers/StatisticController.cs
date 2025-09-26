using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StatisticModule.Controllers;

[ApiController]
[Route("statistic")]
public class StatisticController(IStatisticService statisticService, IAdminService adminService) : ControllerBase
{
    [HttpGet("general/current-season")]
    public async Task<IActionResult> GetGeneralStatisticForCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var statisticDto = await statisticService.GetGeneralStatisticBySeasonId(activeSeason.Id);

        return Ok(statisticDto);
    }
    
    [HttpGet("general/current-season/user/{userId}")]
    public async Task<IActionResult> GetGeneralStatisticForCurrentSeasonByUserId(int userId)
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var statisticDto = await statisticService.GetGeneralStatisticBySeasonId(activeSeason.Id);

        return Ok(statisticDto);
    }
}