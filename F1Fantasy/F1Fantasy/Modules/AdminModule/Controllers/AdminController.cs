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
public class AdminController(
    IUserService userService,
    IAuthorizationService authorizationService,
    IAdminService adminService,
    ISeasonService seasonService)
    : ControllerBase
{
    private readonly IAuthorizationService _authorizationService = authorizationService;

    [HttpPut("user/{userId}/update-roles")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> UpdateUserRoles(int userId, [FromBody] ApplicationUserForAdminUpdateDto dto)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        
        ApplicationUserForAdminGetDto updatedUser = await adminService.UpdateUserRoleAsync(userId, dto.Roles);
        return Ok(updatedUser);
    }

    [HttpPost("season/start/{year}")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> StartSeason(int year)
    {
        StaticDataModule.Dtos.SeasonDto seasonDto = await seasonService.GetSeasonByYearAsync(year);
        if (seasonDto == null)
        {
            return NotFound($"Season with year {year} does not exist.");
        }
        
        SeasonDto startSeason = await adminService.StartSeasonAsync(year);
        return Ok(startSeason);
    }
    
    [HttpGet("season/active")]
    public async Task<IActionResult> GetActiveSeasonAsync()
    {
        SeasonDto? seasonDto = await adminService.GetActiveSeasonAsync();
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
        SeasonDto seasonDto = await adminService.GetActiveSeasonAsync();
        if (seasonDto == null)
        {
            return NotFound($"There is no active season currently");
        }

        await adminService.DeactivateActiveSeasonAsync();
        
        return Ok();
    }
}