using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StatisticModule.Controllers;
[ApiController]
[Route("statistic/public/current-season/players")]
public class UserStatisticController(IUserStatisticService userStatisticService, IAdminService adminService) : ControllerBase
{
    [HttpGet("total-points-scored")]
    public async Task<IActionResult> GetConstructorsWithTotalPointScoredInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await userStatisticService.GetTopUsersInSeasonByTotalFantasyPointsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    
    [HttpGet("average-points-scored")]
    public async Task<IActionResult> GetConstructorsWithAveragePointScoredInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await userStatisticService.GetTopUsersInSeasonByAverageFantasyPointsAsync(activeSeason.Id);

        return Ok(dtos);
    }
}