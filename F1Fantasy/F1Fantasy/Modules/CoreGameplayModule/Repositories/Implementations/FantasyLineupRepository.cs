using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.CoreGameplayModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.CoreGameplayModule.Repositories.Implementations;

public class FantasyLineupRepository(WooF1Context context, IConfiguration configuration) : IFantasyLineupRepository
{
    public async Task<FantasyLineup?> GetFantasyLineupByIdAsync(int fantasyLineupId)
    {
        return await context.FantasyLineups
            .Include(fl => fl.FantasyLineupDrivers)
                .ThenInclude(fld => fld.Driver)
            .Include(fl => fl.FantasyLineupConstructors)
                .ThenInclude(flc => flc.Constructor)
            .Include(fl => fl.PowerupFantasyLineups)
                .ThenInclude(pfl => pfl.Powerup)
            .FirstOrDefaultAsync(fl => fl.Id == fantasyLineupId);
    }
    public async Task<FantasyLineup?> GetFantasyLineupByIdAsTrackingAsync(int fantasyLineupId)
    {
        return await context.FantasyLineups
            .Include(fl => fl.FantasyLineupDrivers)
            .ThenInclude(fld => fld.Driver)
            .Include(fl => fl.FantasyLineupConstructors)
            .ThenInclude(flc => flc.Constructor)
            .Include(fl => fl.PowerupFantasyLineups)
            .ThenInclude(pfl => pfl.Powerup)
            .AsTracking()
            .FirstOrDefaultAsync(fl => fl.Id == fantasyLineupId);
    }
    public async Task<FantasyLineup?> GetFantasyLineupByUserIdAndRaceIdAsync(int userId, int raceId)
    {
        return await context.FantasyLineups
            .Include(fl => fl.FantasyLineupDrivers)
                .ThenInclude(fld => fld.Driver)
            .Include(fl => fl.FantasyLineupConstructors)
                .ThenInclude(flc => flc.Constructor)
            .Include(fl => fl.PowerupFantasyLineups)
                .ThenInclude(pfl => pfl.Powerup)
            .FirstOrDefaultAsync(fl => fl.UserId == userId && fl.RaceId == raceId);
    }
    public async Task<FantasyLineup?> GetCurrentFantasyLineupByUserIdAsync(int userId)
    {
        // Get the active season
        Season? activeSeason = await context.Seasons.FirstOrDefaultAsync(s => s.IsActive);
        if (activeSeason == null)
        {
            return null;
        }

        // Get the fantasy lineup for the user in the active season including related drivers and constructors
        return await context.FantasyLineups
            .Include(fl => fl.FantasyLineupDrivers)
                .ThenInclude(fld => fld.Driver)
            .Include(fl => fl.FantasyLineupConstructors)
                .ThenInclude(flc => flc.Constructor)
            .Include(fl => fl.PowerupFantasyLineups)
                .ThenInclude(pfl => pfl.Powerup)
            .Where(fl => fl.UserId == userId 
                         && fl.Race.SeasonId == activeSeason.Id 
                         && fl.Race.RaceDate > DateOnly.FromDateTime(DateTime.Now))
            .OrderBy(fl => fl.Race.RaceDate)
            .FirstOrDefaultAsync();
        
    }
    
    public async Task<FantasyLineup> UpdateFantasyLineupAsync(
        List<int> driverIds, 
        List<int> constructorIds, 
        List<int> powerupIds, 
        FantasyLineup trackedFantasyLineup,
        int maxDrivers,
        int maxConstructors)
    {
        RemoveUnselectedLineupItems(trackedFantasyLineup, driverIds, constructorIds, powerupIds);
        AddNewItemsAndCalculateTransfers(trackedFantasyLineup, driverIds, constructorIds, powerupIds, maxDrivers, maxConstructors);
        await context.SaveChangesAsync();
        return trackedFantasyLineup;
    }
    
    private void RemoveUnselectedLineupItems(
        FantasyLineup trackedFantasyLineup,
        List<int> driverIds,
        List<int> constructorIds,
        List<int> powerupIds)
    {
        context.FantasyLineupDrivers.RemoveRange(
            context.FantasyLineupDrivers
                .Where(fld => fld.FantasyLineupId == trackedFantasyLineup.Id && !driverIds.Contains(fld.DriverId))
        );
        context.FantasyLineupConstructors.RemoveRange(
            context.FantasyLineupConstructors
                .Where(flc => flc.FantasyLineupId == trackedFantasyLineup.Id && !constructorIds.Contains(flc.ConstructorId))
        );
        context.PowerupFantasyLineups.RemoveRange(
            context.PowerupFantasyLineups
                .Where(pfl => pfl.FantasyLineupId == trackedFantasyLineup.Id && !powerupIds.Contains(pfl.PowerupId))
        );
    }

    private void AddNewItemsAndCalculateTransfers(
        FantasyLineup trackedFantasyLineup,
        List<int> driverIds,
        List<int> constructorIds,
        List<int> powerupIds,
        int maxDrivers,
        int maxConstructors)
    {
        // Add new drivers, constructors, and powerups which are not already in the lineup
        var existingDriverIds = trackedFantasyLineup.FantasyLineupDrivers.Select(fld => fld.DriverId).ToList();
        var existingConstructorIds = trackedFantasyLineup.FantasyLineupConstructors.Select(flc => flc.ConstructorId).ToList();
        var existingPowerupIds = trackedFantasyLineup.PowerupFantasyLineups.Select(pfl => pfl.PowerupId).ToList();
        
        var newDriverIds = driverIds.Except(existingDriverIds).ToList();
        var newConstructorIds = constructorIds.Except(existingConstructorIds).ToList();
        var newPowerupIds = powerupIds.Except(existingPowerupIds).ToList();
        
        // Only count driver transfers if replacing an existing driver, not filling a blank spot
        int blankDriverSpots = Math.Max(0, maxDrivers - existingDriverIds.Count);
        int driverTransfers = Math.Max(0, newDriverIds.Count - blankDriverSpots);

        // Only count constructor transfers if replacing an existing constructor, not filling a blank spot
        int blankConstructorSpots = Math.Max(0, maxConstructors - existingConstructorIds.Count);
        int constructorTransfers = Math.Max(0, newDriverIds.Count - blankConstructorSpots);
        
        int transferCount = driverTransfers + constructorTransfers;
        trackedFantasyLineup.TransfersMade += transferCount;

        
        foreach (var driverId in newDriverIds)
        {
            trackedFantasyLineup.FantasyLineupDrivers.Add(new FantasyLineupDriver
            {
                FantasyLineupId = trackedFantasyLineup.Id,
                DriverId = driverId
            });
        }
        
        foreach (var constructorId in newConstructorIds)
        {
            trackedFantasyLineup.FantasyLineupConstructors.Add(new FantasyLineupConstructor
            {
                FantasyLineupId = trackedFantasyLineup.Id,
                ConstructorId = constructorId
            });
        }
        
        foreach (var powerupId in newPowerupIds)
        {
            trackedFantasyLineup.PowerupFantasyLineups.Add(new PowerupFantasyLineup
            {
                FantasyLineupId = trackedFantasyLineup.Id,
                PowerupId = powerupId
            });
        }
    }
}