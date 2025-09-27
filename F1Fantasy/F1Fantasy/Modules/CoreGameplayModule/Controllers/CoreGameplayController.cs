using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.CoreGameplayModule.Controllers;

[ApiController]
[Route("core")]
public class CoreGameplayController(
    IAuthorizationService authorizationService,
    ICoreGameplayService coreGameplayService)
    : ControllerBase
{
    [HttpGet("fantasy-lineup/{fantasyLineupId:int}")]
    public async Task<IActionResult> GetFantasyLineupById(int fantasyLineupId)
    {
        var fantasyLineupDto = await coreGameplayService.GetFantasyLineupByIdAsync(fantasyLineupId);

        return Ok(fantasyLineupDto);
    }
    
    [HttpGet("fantasy-lineup/race/{raceId:int}/user/{userId:int}")]
    public async Task<IActionResult> GetFantasyLineupByUserIdAndRaceId(int userId, int raceId)
    {
        var fantasyLineupDto = await coreGameplayService.GetFantasyLineupByUserIdAndRaceIdAsync(userId, raceId);

        return Ok(fantasyLineupDto);
    }
    
    [HttpGet("fantasy-lineup/{userId:int}/current")]
    public async Task<IActionResult> GetCurrentFantasyLineupByUserId(int userId)
    {
        var fantasyLineupDto = await coreGameplayService.GetCurrentFantasyLineupByUserIdAsync(userId);

        return Ok(fantasyLineupDto);
    }
    
    [HttpPut("fantasy-lineup/{userId:int}/current")]
    public async Task<IActionResult> UpdateCurrentFantasyLineupByUserId(int userId, [FromBody] Dtos.Update.FantasyLineupDto fantasyLineupDto)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var fantasyLineupGetDto = await coreGameplayService.GetCurrentFantasyLineupByUserIdAsync(userId);
        if (fantasyLineupGetDto.UserId != userId)
        {
            return Forbid();
        }
        
        fantasyLineupDto.Id = fantasyLineupGetDto.Id;
        var updatedFantasyLineupDto = await coreGameplayService.UpdateFantasyLineupAsync(fantasyLineupDto);
        return Ok(updatedFantasyLineupDto);
    }
    
        
    [HttpGet("race/latest-finished")]
    public async Task<IActionResult> GetLatestFinishedRace()
    {
        var raceDto = await coreGameplayService.GetLatestFinishedRaceAsync();

        return Ok(raceDto);
    }
    
            
    [HttpGet("race/latest-finished/result")]
    public async Task<IActionResult> GetLatestFinishedRaceResult()
    {
        var raceDto = await coreGameplayService.GetLatestFinishedRaceResultAsync();

        return Ok(raceDto);
    }
    
    [HttpGet("race/current")]
    public async Task<IActionResult> GetCurrentRace()
    {
        var raceDto = await coreGameplayService.GetCurrentRaceAsync();

        return Ok(raceDto);
    }
    
    [HttpGet("race/latest")]
    public async Task<IActionResult> GetLatestRace()
    {
        var raceDto = await coreGameplayService.GetLatestRaceAsync();

        return Ok(raceDto);
    }


    [HttpGet("powerup/used/user/{userId:int}")]
    public async Task<IActionResult> GetAllUsedPowerupInCurrentSeasonBeforeCurrentRace(int userId)
    {
        var powerupDtos = await coreGameplayService.GetUsedPowerupsBeforeCurrentRaceByUserInASeasonAsync(userId);

        return Ok(powerupDtos);
    }
}