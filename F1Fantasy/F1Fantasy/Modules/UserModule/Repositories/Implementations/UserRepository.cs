using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.UserModule.Repositories.Implementations;

public class UserRepository(WooF1Context context) : IUserRepository
{
    public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
    {
        ApplicationUser existingUser = await context.Users.AsTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
        if(existingUser == null)
        {
            throw new NotFoundException($"User with ID {user.Id} not found.");
        }
        // Update only the properties ưe want to allow

        existingUser.DisplayName = user.DisplayName;
        existingUser.DateOfBirth = user.DateOfBirth;
        existingUser.AcceptNotification = user.AcceptNotification;
        existingUser.ConstructorId = user.ConstructorId;
        existingUser.DriverId = user.DriverId;
        existingUser.CountryId = user.CountryId;

        await context.SaveChangesAsync();
        
        return await context.Users
            .Include(u => u.Constructor)
            .Include(u => u.Driver)
            .Include(u => u.Country)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        

    }

    public async Task<ApplicationUser?> GetUserByIdAsync(int id)
    {
        return await context.Users
            .Include(u => u.Constructor)
            .Include(u => u.Driver)
            .Include(u => u.Country)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<ApplicationUser>> FindUserByDisplayNameAsync(string name)
    {
        return await context.Users
            .Include(u => u.Constructor)
            .Include(u => u.Driver)
            .Include(u => u.Country)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .Where(user => user.DisplayName != null && user.DisplayName.ToLower().Contains(name.ToLower()))
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<(List<ApplicationUser> Users, int TotalCount)> SearchUsersAsync(string query, int skip, int take)
    {
        var userQuery = context.Users
            .Where(d =>
                EF.Functions.ILike(d.DisplayName, $"%{query}%") ||
                EF.Functions.ILike(d.Email, $"%{query}%"));

        var totalCount = await userQuery.CountAsync();
        var users = await userQuery
            .OrderBy(d => d.DisplayName)
            .Skip(skip)
            .Take(take)
            .Include(u => u.Constructor)
            .Include(u => u.Driver)
            .Include(u => u.Country)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .ToListAsync();

        return (users, totalCount);
    }
}