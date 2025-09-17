using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.UserModule.Dtos;
using F1Fantasy.Modules.UserModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.UserModule.Controllers;

[ApiController]
[Route("api/user")]
public class UserDetailController(IUserService userService, IAuthorizationService authorizationService)
    : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("{userId}/update")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] Dtos.Update.ApplicationUserDto userUpdateDto)
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
        
        var updatedUser = await userService.UpdateUserAsync(userUpdateDto);
        return Ok(updatedUser);
    }

    [HttpGet("search")]
    public async Task<IActionResult> FindUserByDisplayName(string name)
    {
        var users = await userService.FindUserByDisplayNameAsync(name);
        return Ok(users);
    }
}