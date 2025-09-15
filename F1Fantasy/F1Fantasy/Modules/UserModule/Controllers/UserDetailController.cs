using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.UserModule.Dtos;
using F1Fantasy.Modules.UserModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.UserModule.Controllers;

[ApiController]
[Route("api/user")]
public class UserDetailController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserService _userService;

    public UserDetailController(IUserService userService, IAuthorizationService authorizationService)
    {
        _userService = userService;
        _authorizationService = authorizationService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
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
        var authResult = await _authorizationService.AuthorizeAsync(User, userId, Policies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var updatedUser = await _userService.UpdateUserAsync(userUpdateDto);
        return Ok(updatedUser);
    }

    [HttpGet("search")]
    public async Task<IActionResult> FindUserByDisplayName(string name)
    {
        var users = await _userService.FindUserByDisplayNameAsync(name);
        return Ok(users);
    }
}