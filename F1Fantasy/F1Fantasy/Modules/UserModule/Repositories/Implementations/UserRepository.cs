using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.UserModule.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly WooF1Context _context;

    public UserRepository(WooF1Context context)
    {
        _context = context;
    }

    public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
    {
        ApplicationUser existingUser = await _context.Users.AsTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
        // Update only the properties ưe want to allow
        if (existingUser != null)
        {
            existingUser.DisplayName = user.DisplayName;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.AcceptNotification = user.AcceptNotification;
            existingUser.ConstructorId = user.ConstructorId;
            existingUser.DriverId = user.DriverId;
            existingUser.CountryId = user.CountryId;

            await _context.SaveChangesAsync();
            
            return await _context.Users
                .Include(u => u.Constructor)
                .Include(u => u.Driver)
                .Include(u => u.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == user.Id);
        }

        throw new InvalidOperationException();
    }

    public async Task<ApplicationUser> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Constructor)
            .Include(u => u.Driver)
            .Include(u => u.Country)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<ApplicationUser>> FindUserByDisplayNameAsync(string name)
    {
        return await _context.Users
            .Include(u => u.Constructor)
            .Include(u => u.Driver)
            .Include(u => u.Country)
            .AsNoTracking()
            .Where(user => user.DisplayName != null && user.DisplayName.ToLower().Contains(name.ToLower()))
            .ToListAsync();
    }

}