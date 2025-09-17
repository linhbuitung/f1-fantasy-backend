using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.CoreGameplayModule.Controllers;

[ApiController]
[Route("api/fantasy-lineup")]
public class CoreGameplayController(
    IAuthorizationService authorizationService,
    ICoreGameplayService coreGameplayService)
    : ControllerBase
{
    [HttpGet("{fantasyLineupId:int}")]
    public async Task<IActionResult> GetFantasyLineupById(int fantasyLineupId)
    {
        var fantasyLineupDto = await coreGameplayService.GetFantasyLineupByIdAsync(fantasyLineupId);

        return Ok(fantasyLineupDto);
    }
    
    [HttpGet("{raceId:int}/user{userId:int}")]
    public async Task<IActionResult> GetFantasyLineupByUserIdAndRaceId(int userId, int raceId)
    {
        var fantasyLineupDto = await coreGameplayService.GetFantasyLineupByUserIdAndRaceIdAsync(userId, raceId);

        return Ok(fantasyLineupDto);
    }
    
    [HttpGet("{userId:int}/current")]
    public async Task<IActionResult> GetCurrentFantasyLineupByUserId(int userId)
    {
        var fantasyLineupDto = await coreGameplayService.GetCurrentFantasyLineupByUserIdAsync(userId);

        return Ok(fantasyLineupDto);
    }
    
    [HttpPut("{userId:int}/current")]
    public async Task<IActionResult> UpdateCurrentFantasyLineupByUserId(int userId, [FromBody] Dtos.Update.FantasyLineupDto fantasyLineupDto)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, Policies.CanOperateOnOwnResource);
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

        var updatedFantasyLineupDto = await coreGameplayService.UpdateFantasyLineupAsync(fantasyLineupDto);
        return Ok(updatedFantasyLineupDto);
    }
}