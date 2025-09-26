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
            .AsSplitQuery()
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
            .AsSplitQuery()
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
            .AsSplitQuery()
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
            .AsSplitQuery()
            .Where(fl => fl.UserId == userId 
                         && fl.Race.SeasonId == activeSeason.Id 
                         && fl.Race.RaceDate > DateOnly.FromDateTime(DateTime.UtcNow))
            .OrderBy(fl => fl.Race.RaceDate)
            .FirstOrDefaultAsync();
        
    }
    
    public async Task<FantasyLineup> UpdateFantasyLineupAsync(
        List<int> driverIds, 
        List<int> constructorIds, 
        List<int> powerupIds, 
        FantasyLineup trackedFantasyLineup,
        int? captainDriverId,
        int maxDrivers,
        int maxConstructors)
    {
        RemoveUnselectedLineupItems(trackedFantasyLineup, driverIds, constructorIds, powerupIds);
        AddNewItemsAndCalculateTransfers(trackedFantasyLineup, driverIds, constructorIds, powerupIds, captainDriverId, maxDrivers, maxConstructors);
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
        List<int> newDriverIds,
        List<int> newConstructorIds,
        List<int> newPowerupIds,
        int? captainDriverId,
        int maxDrivers,
        int maxConstructors)
    {
        // Add new drivers, constructors, and powerups which are not already in the lineup
        var existingDriverIds = trackedFantasyLineup.FantasyLineupDrivers.Select(fld => fld.DriverId).ToList();
        var existingConstructorIds = trackedFantasyLineup.FantasyLineupConstructors.Select(flc => flc.ConstructorId).ToList();
        var existingPowerupIds = trackedFantasyLineup.PowerupFantasyLineups.Select(pfl => pfl.PowerupId).ToList();
        
        // Count driver replacements (drivers removed from old lineup)
        var replacedDrivers = existingDriverIds.Except(newDriverIds).ToList();
        var driverTransfers = replacedDrivers.Count;

        // Count constructor replacements
        var replacedConstructors = existingConstructorIds.Except(newConstructorIds).ToList();
        var constructorTransfers = replacedConstructors.Count;
        
        // Get replaced powerups (powerups removed from old lineup)
        var replacedPowerups = existingPowerupIds.Except(newPowerupIds).ToList();

        // Total transfer count
        var transferCount = driverTransfers + constructorTransfers;
        trackedFantasyLineup.TransfersMade += transferCount;

        #region Remove old drivers, constructors, and powerups which are not in the new lists
        context.FantasyLineupDrivers.RemoveRange(
            context.FantasyLineupDrivers
                .Where(fld => fld.FantasyLineupId == trackedFantasyLineup.Id && replacedDrivers.Contains(fld.DriverId))
        );

        context.FantasyLineupConstructors.RemoveRange(
            context.FantasyLineupConstructors
                .Where(flc => flc.FantasyLineupId == trackedFantasyLineup.Id && replacedConstructors.Contains(flc.ConstructorId))
        );
        
        context.PowerupFantasyLineups.RemoveRange(
            context.PowerupFantasyLineups
                .Where(pfl => pfl.FantasyLineupId == trackedFantasyLineup.Id && replacedPowerups.Contains(pfl.PowerupId))
        );
        #endregion
        
        #region Add new drivers, constructors, and powerups which are added in the new lists

        foreach (var driverId in newDriverIds.Except(existingDriverIds))
        {
            trackedFantasyLineup.FantasyLineupDrivers.Add(new FantasyLineupDriver
            {
                FantasyLineupId = trackedFantasyLineup.Id,
                DriverId = driverId,
                IsCaptain = false
            });
        }
        
        foreach (var constructorId in newConstructorIds.Except(existingConstructorIds))
        {
            trackedFantasyLineup.FantasyLineupConstructors.Add(new FantasyLineupConstructor
            {
                FantasyLineupId = trackedFantasyLineup.Id,
                ConstructorId = constructorId
            });
        }
        
        foreach (var powerupId in  newPowerupIds.Except(existingPowerupIds))
        {
            trackedFantasyLineup.PowerupFantasyLineups.Add(new PowerupFantasyLineup
            {
                FantasyLineupId = trackedFantasyLineup.Id,
                PowerupId = powerupId
            });
        }

        #endregion

        #region Add captain

        if (captainDriverId != null)
        {
            foreach (var fantasyLineupDriver in trackedFantasyLineup.FantasyLineupDrivers)
            {
                fantasyLineupDriver.IsCaptain = fantasyLineupDriver.DriverId == captainDriverId;
            }
        }
        #endregion
    }

    public async Task ResetFantasyLineupsBySeasonAsync(Season season)
    {
        // Remove all connections in FantasyLineupDrivers, FantasyLineupConstructors, PowerupFantasyLineups for all fantasy lineups in the given season
        // Get all fantasy lineup IDs for the given season
        var fantasyLineupIds = await context.FantasyLineups
            .Where(fl => fl.Race.SeasonId == season.Id)
            .Select(fl => fl.Id)
            .ToListAsync();

        // Remove all related drivers, constructors, and powerups
        context.FantasyLineupDrivers.RemoveRange(
            context.FantasyLineupDrivers.Where(fld => fantasyLineupIds.Contains(fld.FantasyLineupId))
        );

        context.FantasyLineupConstructors.RemoveRange(
            context.FantasyLineupConstructors.Where(flc => fantasyLineupIds.Contains(flc.FantasyLineupId))
        );

        context.PowerupFantasyLineups.RemoveRange(
            context.PowerupFantasyLineups.Where(pfl => fantasyLineupIds.Contains(pfl.FantasyLineupId))
        );

        // Reset variables 
        await context.FantasyLineups
            .Where(fl => fantasyLineupIds.Contains(fl.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(fl => fl.TransfersMade, 0)
                .SetProperty(fl => fl.TotalAmount, 0)
                .SetProperty(fl => fl.PointsGained, 0)
            );

        // Save changes
        await context.SaveChangesAsync();
    }
}