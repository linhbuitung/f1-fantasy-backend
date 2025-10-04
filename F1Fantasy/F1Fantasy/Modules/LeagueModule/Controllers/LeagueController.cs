using F1Fantasy.Core.Common;
using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Modules.LeagueModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.LeagueModule.Controllers;

[ApiController]
[Route("user/{userId:int}")]
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
    
    [HttpPost("league/{leagueId:int}/join")]
    public async Task<IActionResult> JoinLeagueById(int userId, int leagueId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        await leagueService.JoinLeagueAsync(leagueId: leagueId, playerId: userId);
        return Ok();
    }
    
    [HttpGet("/league/{leagueId:int}/")]
    public async Task<IActionResult> GetLeagueById(int leagueId, int pageNum = 1, int pageSize = 10)
    {
        var league = await leagueService.GetLeagueByIdAsync(leagueId, pageNum, pageSize);
        return Ok(league);
    }
    
    [HttpGet("/owner/{ownerId:int}/league/{leagueId:int}/join-requests")]
    public async Task<IActionResult> GetJoinRequestByLeagueIdAndOwnerId( int ownerId, int leagueId)
    {        
        var authResult = await authorizationService.AuthorizeAsync(User, ownerId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        var league = await leagueService.GetLeagueByIdAsync(leagueId, pageNum: 1, pageSize: 1);
        if (league.Owner!.Id != ownerId)
        {
            return Forbid();
        }
        var userLeagues = await leagueService.GetUnAcceptedUserLeagueByLeagueIdAsync(leagueId);
        return Ok(userLeagues);
    }
    
    /*
    [HttpGet("/league/{leagueId:int}/waiting-requests")]
    public async Task<IActionResult> GetJoinRequestsInLeagueById(int leagueId)
    {
        var userLeagues = await leagueService.GetAllWaitingJoinRequestsAsync(leagueId);
        return Ok(userLeagues);
    }
    */
    
    [HttpPut("league/{leagueId:int}/handle-join-request")]
    public async Task<IActionResult> HandleJoinLeagueById(int userId, int leagueId, [FromBody] Dtos.Update.UserLeagueDto userLeagueDto)
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
        if (leagueDto.Owner!.Id != userId)
        {
            return Forbid();
        }
        
        var userLeague = await leagueService.HandleJoinRequestAsync(userLeagueDto);
        return Ok(userLeague);
    }
    
    [HttpPut("league/{leagueId:int}")]
    public async Task<IActionResult> UpdateLeagueById(int userId, int leagueId, [FromBody] Dtos.Update.LeagueDto leagueDto)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded || leagueId != leagueDto.Id)
        {
            return Forbid();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var league = await leagueService.UpdateLeagueAsync(leagueDto);
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
        await leagueService.RemovePlayerFromLeagueAsync(leagueId, userId);
        return Ok();
    }

    [HttpDelete("league/{leagueId:int}/kick/{playerId:int}")]
    public async Task<IActionResult> KickPlayerFromLeague(int userId, int leagueId, int playerId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        var leagueDto = await leagueService.GetLeagueByIdAsync(leagueId, 1, 1);
        if (leagueDto.Owner!.Id != userId)
        {
            return Forbid();
        }
        await leagueService.RemovePlayerFromLeagueAsync(leagueId, playerId);
        return Ok();
    }
    
    [HttpGet("/league/full-text-search")]
    public async Task<IActionResult> SearchLeagues(
        [FromQuery] string query,
        [FromQuery] int pageNum = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required.");

        var result = await leagueService.SearchLeaguesAsync(query, pageNum, pageSize);
        return Ok(result);
    }

    [HttpGet("league/joined")]
    public async Task<IActionResult> GetLeaguesByUserId(int userId)
    {
        var leagues = await leagueService.GetJoinedLeaguesByUserIdAsync(userId);
        return Ok(leagues);
    }

    [HttpGet("league/owned")]
    public async Task<IActionResult> GetOwnedLeaguesByUserId(int userId)
    {
        var leagues = await leagueService.GetOwnedLeaguesByUserIdAsync(userId);
        return Ok(leagues);
    }
    
    [HttpGet("league/{leagueId:int}/join-request")]
    public async Task<IActionResult> GetJoinRequestByPlayerIdAndLeagueId(int userId, int leagueId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        var userLeague = await leagueService.GetUserLeagueByIdAsync(leagueId, userId);
        return Ok(userLeague);
    }
}