using F1Fantasy.Core.Common;
using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Modules.LeagueModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.LeagueModule.Controllers;

[ApiController]
[Route("admin/{userId:int}")]
public class AdminLeagueController(
    IAuthorizationService authorizationService,
    ILeagueService leagueService)
    : ControllerBase
{
    [HttpPost("league/public")]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]    
    public async Task<IActionResult> CreatePublicLeague(int userId, [FromBody]Dtos.Create.LeagueDto leagueCreateDto)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        leagueCreateDto.OwnerId = userId;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var league = await leagueService.AddLeagueAsync(leagueCreateDto, LeagueType.Public);

        return Ok(league);
    }
    
    [HttpDelete("league/{leagueId:int}")]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
    public async Task<IActionResult> DeleteLeagueById(int leagueId)
    {
        await leagueService.DeleteLeagueByIdAsync(leagueId);
        return Ok();
    }
}