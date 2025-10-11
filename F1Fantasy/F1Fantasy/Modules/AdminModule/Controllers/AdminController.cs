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
[Route("admin")]
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
    public async Task<IActionResult> UpdateUserRoles(int userId, [FromBody] Dtos.Update.ApplicationUserForAdminDto dto)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user.Roles.Contains(AppRoles.SuperAdmin))
        {
            return Forbid("You cannot update superadmin");
        }
        Dtos.Get.ApplicationUserForAdminDto updatedUser = await adminService.UpdateUserRoleAsync(userId, dto.Roles);
        return Ok(updatedUser);
    }

    [HttpPost("season/start/{year}")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> StartSeason(int year)
    {
        StaticDataModule.Dtos.SeasonDto seasonDto = await seasonService.GetSeasonByYearAsync(year);
        
        SeasonDto startSeason = await adminService.StartSeasonAsync(year);
        return Ok(startSeason);
    }
    
    [HttpGet("season/active")]
    public async Task<IActionResult> GetActiveSeasonAsync()
    {
        SeasonDto? seasonDto = await adminService.GetActiveSeasonAsync();
        
        return Ok(seasonDto);
    }
    
    [HttpPost("season/active/deactivate")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> DeactivateActiveSeasonAsync()
    {
        SeasonDto seasonDto = await adminService.GetActiveSeasonAsync();

        await adminService.DeactivateActiveSeasonAsync();
        
        return Ok();
    }

    [HttpGet("pickable-items")] public async Task<IActionResult> GetPickableItemsAsync()
    {
        var pickableItemDto = await adminService.GetPickableItemAsync();        
        return Ok(pickableItemDto);
    }
    
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
    [HttpPut("pickable-items")] 
    public async Task<IActionResult> UpdatePickableItemsAsync([FromBody] Dtos.Update.PickableItemDto dto)
    {
        _ = await adminService.GetPickableItemAsync();        
        var pickableItemDto = await adminService.UpdatePickableItemAsync(dto);
        return Ok(pickableItemDto);
    }
    
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
    [HttpPut("pickable-items/{seasonYear:int}")] 
    public async Task<IActionResult> UpdatePickableItemsAsync(int seasonYear)
    {
        _ = await adminService.GetPickableItemAsync();        
        var pickableItemDto = await adminService.UpdatePickableItemFromAllDriversInASeasonYearAsync(seasonYear);
        return Ok(pickableItemDto);
    }
    
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
    [HttpPatch("driver/{driverId:int}")] 
    public async Task<IActionResult> UpdateDriverInfoAsync(int driverId, [FromBody] Dtos.Update.DriverDto dto)
    {
        if(!ModelState.IsValid || driverId != dto.Id)
        {
            return BadRequest(ModelState);
        }
        
        var resultDto = await adminService.UpdateDriverInfoAsync(dto);
        return Ok(resultDto);
    }

    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
    [HttpPatch("constructor/{constructorId:int}")]
    public async Task<IActionResult> UpdateConstructorInfoAsync(int constructorId,
        [FromBody] Dtos.Update.ConstructorDto dto)
    {
        if (!ModelState.IsValid || constructorId != dto.Id)
        {
            return BadRequest(ModelState);
        }
        
        var resultDto = await adminService.UpdateConstructorInfoAsync(dto);
        return Ok(resultDto);
    }
    
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
    [HttpPatch("circuit/{circuitId:int}")]
    public async Task<IActionResult> UpdateCircuitInfoAsync(int circuitId,
        [FromBody] Dtos.Update.CircuitDto dto)
    {
        if (!ModelState.IsValid || circuitId != dto.Id)
        {
            return BadRequest(ModelState);
        }
        
        var resultDto = await adminService.UpdateCircuitInfosync(dto);
        return Ok(resultDto);
    }
    
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
    [HttpPatch("powerup/{powerupId:int}")]
    public async Task<IActionResult> UpdateCircuitInfoAsync(int powerupId,
        [FromBody] Dtos.Update.PowerupDto dto)
    {
        if (!ModelState.IsValid || powerupId != dto.Id)
        {
            return BadRequest(ModelState);
        }
        
        var resultDto = await adminService.UpdatePowerupInfoAsync(dto);
        return Ok(resultDto);
    }
}