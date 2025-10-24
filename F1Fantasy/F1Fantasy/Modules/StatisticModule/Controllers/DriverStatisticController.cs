using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StatisticModule.Controllers;

// Specific endpoints for statistic routes
[ApiController]
[Route("statistic/public/current-season/drivers/")]
public class DriverStatisticController(IDriverStatisticService driverStatisticService, IAdminService adminService) : ControllerBase
{
    [HttpGet("total-points-scored")]
    public async Task<IActionResult> GetDriversWithTotalPointScoredInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await driverStatisticService.GetTopDriversInSeasonByTotalFantasyPointsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    
    [HttpGet("average-points-scored")]
    public async Task<IActionResult> GetDriversWithAveragePointScoredInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await driverStatisticService.GetTopDriversInSeasonByAverageFantasyPointsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    
    [HttpGet("selection-percentage")]
    public async Task<IActionResult> GetDriversWithSelectionPercentageInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await driverStatisticService.GetTopDriversInSeasonBySelectionPercentageAsync(activeSeason.Id);

        return Ok(dtos);
    }

    [HttpGet("race-wins")]
    public async Task<IActionResult> GetDriversWithRaceWinsInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await driverStatisticService.GetTopDriversInSeasonByTotalRacesWinsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    [HttpGet("podiums")]
    public async Task<IActionResult> GetDriversWithPodiumsInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await driverStatisticService.GetTopDriversInSeasonByTotalPodiumsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    [HttpGet("top-10-finishes")]
    public async Task<IActionResult> GetDriversWithTop10FinishesnInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await driverStatisticService.GetTopDriversInSeasonByTotalTop10FinishesAsync(activeSeason.Id);

        return Ok(dtos);
    }
    [HttpGet("fastest-laps")]
    public async Task<IActionResult> GetDriversWithFastestLapsInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await driverStatisticService.GetTopDriversInSeasonByTotalFastestLapsAsync(activeSeason.Id);

        return Ok(dtos);
    }
    [HttpGet("dnfs")]
    public async Task<IActionResult> GetDriversWithDnfsInCurrentSeason()
    {
        var activeSeason = await adminService.GetActiveSeasonAsync();
        
        var dtos = await driverStatisticService.GetTopDriversInSeasonByTotalDnfsAsync(activeSeason.Id);

        return Ok(dtos);
    }
}