using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AdminModule.Dtos;
using F1Fantasy.Modules.AdminModule.Repositories.Interfaces;
using F1Fantasy.Modules.AuthModule.Extensions;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.AdminModule.Repositories.Implementations;

public class AdminRepository : IAdminRepository
{
    private readonly WooF1Context _context;

    public AdminRepository(WooF1Context context)
    {
        _context = context;
    }
    
    public async Task<Season> UpdateSeasonStatusAsync(int year, bool isActive)
    {
        Season season = await _context.Seasons.AsTracking().FirstOrDefaultAsync(s => s.Year == year);
        if (season == null)
        {
            throw new InvalidOperationException($"Season with year {year} does not exist.");
        } 
        season.IsActive = isActive;

        await _context.SaveChangesAsync();
        return season;
    }
    
    public async Task<Season> GetActiveSeasonAsync()
    {
        return await _context.Seasons.AsNoTracking().FirstOrDefaultAsync(s => s.IsActive);
    }

    public async Task<List<ApplicationRole>> GetAllRolesAsync()
    {
        return await _context.Roles.AsNoTracking().ToListAsync();
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
        List<ApplicationRole> availableRoles = await _context.Roles.AsNoTracking().ToListAsync();
        var selectedRoles = availableRoles.Where(r => roleNames.Contains(r.Name)).ToList();

        // Remove all existing roles 
        List<ApplicationUserRole> userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
        _context.UserRoles.RemoveRange(userRoles);

        // Add missing roles 
        foreach (var role in selectedRoles)
        {
            _context.UserRoles.Add(new ApplicationUserRole { UserId = userId, RoleId = role.Id });
        }

        await _context.SaveChangesAsync();
        return await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);    
    }
}