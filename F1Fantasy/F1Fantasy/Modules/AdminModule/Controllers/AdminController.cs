using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.AdminModule.Dtos;
using F1Fantasy.Modules.AdminModule.Repositories.Interfaces;
using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using F1Fantasy.Modules.UserModule.Dtos;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;
using F1Fantasy.Modules.UserModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.AdminModule.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserService _userService;
    private readonly IAdminService _adminService;
    private readonly ISeasonService _seasonService;

    public AdminController(
        IUserService userService,
        IAuthorizationService authorizationService,
        IAdminService adminService,
        ISeasonService seasonService)
    {
        _userService = userService;
        _authorizationService = authorizationService;
        _adminService = adminService;
        _seasonService = seasonService;
    }

    [HttpPut("user/{userId}/update-roles")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> UpdateUserRoles(int userId, [FromBody] ApplicationUserForAdminUpdateDto dto)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        
        ApplicationUserForAdminGetDto updatedUser = await _adminService.UpdateUserRoleAsync(userId, dto.Roles);
        return Ok(updatedUser);
    }

    [HttpPost("season/start/{year}")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> StartSeason(int year)
    {
        StaticDataModule.Dtos.SeasonDto seasonDto = await _seasonService.GetSeasonByYearAsync(year);
        if (seasonDto == null)
        {
            return NotFound($"Season with year {year} does not exist.");
        }
        
        SeasonDto startSeason = await _adminService.StartSeasonAsync(year);
        return Ok(startSeason);
    }
    
    [HttpGet("season/active")]
    public async Task<IActionResult> GetActiveSeasonAsync()
    {
        SeasonDto seasonDto = await _adminService.GetActiveSeasonAsync();
        if (seasonDto == null)
        {
            return NotFound($"There is no active season currently");
        }
        
        return Ok(seasonDto);
    }
    
    [HttpPost("season/active/deactivate")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> DeactivateActiveSeasonAsync()
    {
        SeasonDto seasonDto = await _adminService.GetActiveSeasonAsync();
        if (seasonDto == null)
        {
            return NotFound($"There is no active season currently");
        }

        await _adminService.DeactivateActiveSeasonAsync();
        
        return Ok();
    }
}