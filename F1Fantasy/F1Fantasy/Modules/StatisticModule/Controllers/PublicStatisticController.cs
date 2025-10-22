using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StatisticModule.Controllers;

// Specific endpoints for statistic routes
[ApiController]
[Route("statistic/public")]
public class PublicStatisticController(IStatisticService statisticService, IAdminService adminService) : ControllerBase
{
    [Authorize]
    [HttpGet("current-season/drivers/total-points-scored")]
    public async Task<IActionResult> GetDriversWithTotalPointScoredInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await statisticService.GetTopDriversInSeasonByFantasyPointsAsync(activeSeason.Id);

        return Ok(dtos);
    }
}