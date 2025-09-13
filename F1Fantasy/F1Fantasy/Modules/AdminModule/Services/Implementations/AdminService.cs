using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AdminModule.Dtos;
using F1Fantasy.Modules.AdminModule.Dtos.Mapper;
using F1Fantasy.Modules.AdminModule.Repositories.Interfaces;
using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace F1Fantasy.Modules.AdminModule.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly WooF1Context _context;
    
    public AdminService( IAdminRepository adminRepository, WooF1Context context)
    {
        _adminRepository = adminRepository;
        _context = context;
    }
    
    public async Task<SeasonDto> StartSeasonAsync(int year)
    {
        Season currentActiveSeason = await _adminRepository.GetActiveSeasonAsync();
        if (currentActiveSeason != null)
        {
            throw new InvalidOperationException("There is already an active season. Please deactivate it before starting a new one.");
        }
        
        Season activeSeason = await _adminRepository.UpdateSeasonStatusAsync(year, isActive: true);
        
        SeasonDto returnDto = AdminDtoMapper.MapSeasonToDto(activeSeason);
        return returnDto;
    }

    public async Task<SeasonDto?> GetActiveSeasonAsync()
    {
        Season? currentlyActiveSeason = await _adminRepository.GetActiveSeasonAsync();
        if (currentlyActiveSeason == null)
        {
            return null;
        }
        SeasonDto returnDto = AdminDtoMapper.MapSeasonToDto(currentlyActiveSeason);
        return returnDto;
    }

    public async Task DeactivateActiveSeasonAsync()
    {
        Season currentlyActiveSeason = await _adminRepository.GetActiveSeasonAsync();
        if (currentlyActiveSeason == null)
        {
            throw new InvalidOperationException("There is no active season.");
        }
        
        // Deactivate all seasons
        await _adminRepository.UpdateSeasonStatusAsync(currentlyActiveSeason.Year, isActive: false);
    }
    public async Task<ApplicationUserForAdminGetDto> UpdateUserRoleAsync(int userId, List<string> roleNames)
    {
        // Always ensure Player role is present
        if (!roleNames.Contains(AppRoles.Player))
            roleNames.Add(AppRoles.Player);
        
        // verify all roles exist
        List<string> notFoundRoles = await VerifyRolesExistAsync(roleNames);
        if (notFoundRoles.Count > 0)
        {
            throw new ArgumentException($"Roles not found in the database: {string.Join(", ", notFoundRoles)}");
        }
        
        ApplicationUser updatedUser = await _adminRepository.UpdateUserRoleAsync(userId, roleNames);

        ApplicationUserForAdminGetDto returnGetDto = AdminDtoMapper.MapUserToApplicationUserForAdminDto(updatedUser, updatedUser.UserRoles.Select(ur => ur.Role).ToList());
        
        return returnGetDto;
    }
    
    // This return list of roles that are not found in the database
    // If all roles exist, it returns an empty list
    private async Task<List<string>> VerifyRolesExistAsync(List<string> roleNames)
    {
        List<ApplicationRole> roles = await _adminRepository.GetAllRolesAsync();
        List<string> notFoundRoles = new List<string>();
        foreach (var roleName in roleNames)
        {
            if (roles.All(r => r.Name != roleName))
            {
                notFoundRoles.Add(roleName);
            }
        }
        return notFoundRoles;
    }
}