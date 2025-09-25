using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.UserModule.Dtos;
using F1Fantasy.Modules.UserModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.UserModule.Controllers;

[ApiController]
[Route("user")]
public class UserDetailController(IUserService userService, IAuthorizationService authorizationService)
    : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    [Authorize]
    [HttpPut("{userId}/update")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] Dtos.Update.ApplicationUserDto userUpdateDto)
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
        
        var updatedUser = await userService.UpdateUserAsync(userUpdateDto);
        return Ok(updatedUser);
    }

    [HttpGet("search")]
    public async Task<IActionResult> FindUserByDisplayName(string name)
    {
        var users = await userService.FindUserByDisplayNameAsync(name);
        return Ok(users);
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await userService.GetUserByIdAsync(userId);
        return Ok(user);
    }
}