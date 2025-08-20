using F1Fantasy.Modules.AdminModule.Dtos;

namespace F1Fantasy.Modules.AdminModule.Services.Interfaces;

public interface IAdminService
{
    Task<SeasonDto>  StartSeasonAsync(int year);
    
    Task<SeasonDto> GetActiveSeasonAsync();

    Task DeactivateActiveSeasonAsync();
    
    Task<ApplicationUserForAdminGetDto> UpdateUserRoleAsync(int userId, List<string> roleNames);
}