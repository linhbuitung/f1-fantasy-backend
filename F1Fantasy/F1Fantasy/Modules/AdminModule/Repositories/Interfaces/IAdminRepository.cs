using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;
using F1Fantasy.Modules.AdminModule.Dtos.Get;

namespace F1Fantasy.Modules.AdminModule.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<Season> UpdateSeasonStatusAsync(int year, bool isActive);
    
    Task<Season?> GetActiveSeasonAsync();
    
    Task<List<ApplicationRole>> GetAllRolesAsync();
    
    Task<ApplicationUser> UpdateUserRoleAsync(int userId, List<string> roleNames);
    
    Task<PickableItem?> GetPickableItemAsync();
    
    Task <Driver> UpdateDriverInfoAsync(Driver driver);
    
    Task<Constructor> UpdateConstructorInfoAsync(Constructor constructor);
    
    Task<Circuit> UpdateCircuitInfoAsync(Circuit circuit);
    
    Task<Powerup> UpdatePowerupInfoAsync(Powerup powerup);
}