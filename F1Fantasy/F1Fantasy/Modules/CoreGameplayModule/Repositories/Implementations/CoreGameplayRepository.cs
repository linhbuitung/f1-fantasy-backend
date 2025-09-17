using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.CoreGameplayModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.CoreGameplayModule.Repositories.Implementations;

public class CoreGameplayRepository(WooF1Context context) : ICoreGameplayRepository
{
    public async Task<PickableItem?> GetPickableItemsAsync()
    {
        // Get all drivers connected to the only instance of PickableItem
        return await context.PickableItems
            .Include(pi => pi.Drivers)
            .Include(pi => pi.Constructors)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Powerup>> GetAllPowerupsAsync()
    {
        return await context.Powerups.AsNoTracking().ToListAsync();
    }

    // Get all powerups which has not been used by the user in the current active season
    public async Task<List<Powerup>> GetAvailablePowerupsByUserIdAndSeasonIdAsync(int userId, int seasonId)
    {
        // Get all powerups used by the user in the current active season
        var usedPowerupIds = context.PowerupFantasyLineups
            .Where(pfl => pfl.FantasyLineup.UserId == userId && pfl.FantasyLineup.Race.SeasonId == seasonId);
        
        // Get all powerups which are not in the usedPowerupIds list
        return await context.Powerups
            .Where(p => !usedPowerupIds.Any(up => up.PowerupId == p.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Powerup>> GetAvailablePowerupsByFantasyLineupIdAsync(int fantasyLineupId)
    {
        var fantasyLineup = await context.FantasyLineups
            .Include(fl => fl.Race)
            .FirstOrDefaultAsync(fl => fl.Id == fantasyLineupId);
        if (fantasyLineup == null)
        {
            throw new NotFoundException("Fantasy lineup not found");
        }
        
        return await GetAvailablePowerupsByUserIdAndSeasonIdAsync(fantasyLineup.UserId, fantasyLineup.Race.SeasonId);
    }
    
    public async Task<List<int>> GetNonExistentDriverIdsAsync(List<int> driverIds)
    {
        var existentDriverIds = await context.Drivers
            .Where(d => driverIds.Contains(d.Id))
            .Select(d => d.Id)
            .ToListAsync();
        
        return driverIds.Except(existentDriverIds).ToList();
    }

    public async Task<List<int>> GetNonExistentConstructorIdsAsync(List<int> constructorIds)
    {
        var existentConstructorIds = await context.Constructors
            .Where(c => constructorIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync();
        
        return constructorIds.Except(existentConstructorIds).ToList();
    }

    public async Task<List<int>> GetNonExistentPowerupIdsAsync(List<int> powerupIds)
    {
        var existentPowerupIds = await context.Powerups
            .Where(p => powerupIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();
        
        return powerupIds.Except(existentPowerupIds).ToList();
    }
}