using F1Fantasy.Core.Common;
using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Modules.LeagueModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.LeagueModule.Controllers;

[ApiController]
[Route("api/user/{userId:int}")]
public class LeagueController(
    IAuthorizationService authorizationService,
    ILeagueService leagueService)
    : ControllerBase
{
    [HttpPost("league/private")]
    [Authorize(Roles = AppRoles.Player)]
    public async Task<IActionResult> CreatePrivateLeague(int userId, [FromBody]Dtos.Create.LeagueDto leagueCreateDto)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, Policies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        leagueCreateDto.OwnerId = userId;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var league = await leagueService.AddLeagueAsync(leagueCreateDto, LeagueType.Private);

        return Ok(league);
    }
    
    [HttpPost("league/public")]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]    
    public async Task<IActionResult> CreatePublicLeague(int userId, [FromBody]Dtos.Create.LeagueDto leagueCreateDto)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, Policies.CanOperateOnOwnResource);
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
    
    [HttpPost("join/{leagueId:int}")]
    public async Task<IActionResult> JoinLeagueById(int userId, int leagueId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, Policies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        await leagueService.JoinLeagueAsync(userId, leagueId);
        return Ok();
    }
    
    [HttpGet("/api/league/{leagueId:int}/")]
    public async Task<IActionResult> GetLeagueById(int leagueId, int pageNum = 1, int pageSize = 10)
    {
        var league = await leagueService.GetLeagueByIdAsync(leagueId, pageNum, pageSize);
        return Ok(league);
    }
    
    [HttpDelete("/api/admin/league/{leagueId:int}")]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
    public async Task<IActionResult> DeleteLeagueById(int leagueId)
    {
        await leagueService.DeleteLeagueByIdAsync(leagueId);
        return Ok();
    }
    
    [HttpDelete("league/{leagueId:int}")]
    public async Task<IActionResult> DeleteLeagueById(int userId, int leagueId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, Policies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        { 
            return Forbid();
        }
        await leagueService.DeleteLeagueByIdAsync(leagueId);
        return Ok();
    }
}