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
    
    Task<Driver?> GetDriverByIdAsync(int driverId);
    Task <Driver> UpdateDriverInfoAsync(Driver driver);
    
    Task<Constructor?> GetConstructorByIdAsync(int constructorId);
    
    Task<Constructor> UpdateConstructorInfoAsync(Constructor constructor);
    
    Task<Circuit?> GetCircuitByIdAsync(int circuitId);
    
    Task<Circuit> UpdateCircuitInfoAsync(Circuit circuit);
    
    Task<Powerup?> GetPowerupByIdAsync(int powerupId);
    
    Task<Powerup> UpdatePowerupInfoAsync(Powerup powerup);
}