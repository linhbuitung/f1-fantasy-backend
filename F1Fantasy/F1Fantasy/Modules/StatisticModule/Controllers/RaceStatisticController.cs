using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StatisticModule.Controllers;

[ApiController]
[Route("statistic/public/current-season/races/")]
public class RaceStatisticController(IRaceStatisticService raceStatisticService) : ControllerBase
{
    [HttpGet("{raceId:int}")]
    public async Task<IActionResult> GetRaceStatisticsByRaceId(int raceId)
    {
        var raceStatisticsDto = await raceStatisticService.GetRaceStatisticByIdAsync(raceId);

        return Ok(raceStatisticsDto);
    }
}