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
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded || userId != leagueCreateDto.OwnerId)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var league = await leagueService.AddLeagueAsync(leagueCreateDto, LeagueType.Private);

        return Ok(league);
    }
    
    [HttpPost("league/join/{leagueId:int}")]
    public async Task<IActionResult> JoinLeagueById(int userId, int leagueId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
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
    
    [HttpGet("league/{leagueId:int}/join-requests")]
    public async Task<IActionResult> GetJoinRequestById( int userId, int leagueId)
    {        
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        var userLeague = await leagueService.GetUserLeagueByIdAsync(leagueId, userId);
        return Ok(userLeague);
    }
    
    [HttpGet("/api/league/{leagueId:int}/waiting-requests")]
    public async Task<IActionResult> GetJoinRequestsInLeagueById(int leagueId)
    {
        var userLeagues = await leagueService.GetAllWaitingJoinRequestsAsync(leagueId);
        return Ok(userLeagues);
    }
    
    [HttpPut("league/{leagueId:int}/handle-join-request")]
    public async Task<IActionResult> JoinLeagueById(int userId, int leagueId, [FromBody] Dtos.Update.UserLeagueDto userLeagueDto)
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
        
        var leagueDto = await leagueService.GetLeagueByIdAsync(leagueId, 1, 1);
        if (leagueDto.Owner.Id != userId)
        {
            return Forbid();
        }
        
        var league = await leagueService.HandleJoinRequestAsync(userLeagueDto);
        return Ok(league);
    }
    
    [HttpDelete("league/{leagueId:int}")]
    public async Task<IActionResult> DeleteLeagueById(int userId, int leagueId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        { 
            return Forbid();
        }
        await leagueService.DeleteLeagueByIdAsync(leagueId);
        return Ok();
    }
    
    [HttpDelete("league/{leagueId:int}/leave")]
    public async Task<IActionResult> LeaveLeagueById(int userId, int leagueId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        { 
            return Forbid();
        }
        await leagueService.LeaveLeagueAsync(leagueId, userId);
        return Ok();
    }

}