using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AdminModule.Dtos;
using F1Fantasy.Modules.AdminModule.Repositories.Interfaces;
using F1Fantasy.Modules.AuthModule.Extensions;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.AdminModule.Repositories.Implementations;

public class AdminRepository(WooF1Context context) : IAdminRepository
{
    public async Task<Season> UpdateSeasonStatusAsync(int year, bool isActive)
    {
        Season season = await context.Seasons.AsTracking().FirstOrDefaultAsync(s => s.Year == year);
        if (season == null)
        {
            throw new InvalidOperationException($"Season with year {year} does not exist.");
        } 
        season.IsActive = isActive;

        await context.SaveChangesAsync();
        return season;
    }
    
    public async Task<Season?> GetActiveSeasonAsync()
    {
        return await context.Seasons.AsNoTracking().FirstOrDefaultAsync(s => s.IsActive);
    }

    public async Task<List<ApplicationRole>> GetAllRolesAsync()
    {
        return await context.Roles.AsNoTracking().ToListAsync();
    }
    public async Task<ApplicationUser> UpdateUserRoleAsync(int userId, List<string> roleNames)
    {
        // Logic for checking existence called in service layer
        /*ApplicationUser user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return null; // User not found
        }*/

        // Get all roles from DB
        List<ApplicationRole> availableRoles = await context.Roles.AsNoTracking().ToListAsync();
        var selectedRoles = availableRoles.Where(r => roleNames.Contains(r.Name)).ToList();

        // Remove all existing roles 
        List<ApplicationUserRole> userRoles = await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
        context.UserRoles.RemoveRange(userRoles);

        // Add missing roles 
        foreach (var role in selectedRoles)
        {
            context.UserRoles.Add(new ApplicationUserRole { UserId = userId, RoleId = role.Id });
        }

        await context.SaveChangesAsync();
        return await context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);    
    }

    public async Task<PickableItem?> GetPickableItemAsync()
    {
        return await context.PickableItems
            .Include(p => p.Drivers)
            .Include(p => p.Constructors)
            .AsTracking().FirstOrDefaultAsync(p => p.Id == 1);
    }

    public async Task<Driver> UpdateDriverInfoAsync(Driver driver)
    {
        Driver existingDriver = await context.Drivers.AsTracking().FirstOrDefaultAsync(d => d.Id == driver.Id);
        
        // Update only the properties we want to allow
        if (existingDriver != null)
        {
            existingDriver.Price = driver.Price;
            existingDriver.ImgUrl = driver.ImgUrl;
            
            await context.SaveChangesAsync();
            
            return await context.Drivers
                .Include(d => d.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == driver.Id);
        }

        throw new InvalidOperationException();
    }

    public async Task<Constructor> UpdateConstructorInfoAsync(Constructor constructor)
    {
        Constructor existingConstructor = await context.Constructors.AsTracking().FirstOrDefaultAsync(c => c.Id == constructor.Id);
        
        // Update only the properties we want to allow
        if (existingConstructor != null)
        {
            existingConstructor.Price = constructor.Price;
            existingConstructor.ImgUrl = constructor.ImgUrl;
            
            await context.SaveChangesAsync();
            
            return await context.Constructors
                .Include(c => c.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == constructor.Id);
        }

        throw new InvalidOperationException();
    }

    public async Task<Circuit> UpdateCircuitInfoAsync(Circuit circuit)
    {
        Circuit existingCircuit = await context.Circuits.AsTracking().FirstOrDefaultAsync(c => c.Id == circuit.Id);
        
        // Update only the properties we want to allow
        if (existingCircuit != null)
        {
            existingCircuit.ImgUrl = circuit.ImgUrl;
            
            await context.SaveChangesAsync();
            
            return await context.Circuits
                .Include(c => c.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == circuit.Id);
        }

        throw new InvalidOperationException();
    }

    public async Task<Powerup> UpdatePowerupInfoAsync(Powerup powerup)
    {
        Powerup existingPowerup = await context.Powerups.AsTracking().FirstOrDefaultAsync(p => p.Id == powerup.Id);
        
        // Update only the properties we want to allow
        if (existingPowerup != null)
        {
            existingPowerup.ImgUrl = powerup.ImgUrl;
            
            await context.SaveChangesAsync();
            
            return await context.Powerups
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == powerup.Id);
        }

        throw new InvalidOperationException();
    }
}