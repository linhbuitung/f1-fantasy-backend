using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Implementations;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StatisticModule.Controllers;

[ApiController]
[Route("statistic/public/current-season/constructors")]
public class ConstructorStatisticController(IConstructorStatisticService constructorStatisticService, IAdminService adminService) : ControllerBase
{
    [HttpGet("total-points-scored")]
    public async Task<IActionResult> GetConstructorsWithTotalPointScoredInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await constructorStatisticService.GetTopConstructorsInSeasonByTotalFantasyPointsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    
    [HttpGet("average-points-scored")]
    public async Task<IActionResult> GetConstructorsWithAveragePointScoredInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await constructorStatisticService.GetTopConstructorsInSeasonByAverageFantasyPointsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    
    [HttpGet("selection-percentage")]
    public async Task<IActionResult> GetConstructorsWithSelectionPercentageInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await constructorStatisticService.GetTopConstructorsInSeasonBySelectionPercentageAsync(activeSeason.Id);

        return Ok(dtos);
    }
    
    [HttpGet("podiums")]
    public async Task<IActionResult> GetConstructorsWithPodiumsInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await constructorStatisticService.GetTopConstructorsInSeasonByTotalPodiumsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    [HttpGet("top-10-finishes")]
    public async Task<IActionResult> GetConstructorsWithTop10FinishesnInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await constructorStatisticService.GetTopConstructorsInSeasonByTotalTop10FinishesAsync(activeSeason.Id);

        return Ok(dtos);
    }
}