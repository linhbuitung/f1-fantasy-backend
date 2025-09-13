using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.AdminModule.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<Season> UpdateSeasonStatusAsync(int year, bool isActive);
    
    Task<Season?> GetActiveSeasonAsync();
    
    Task<List<ApplicationRole>> GetAllRolesAsync();
    
    Task<ApplicationUser> UpdateUserRoleAsync(int userId, List<string> roleNames);
}