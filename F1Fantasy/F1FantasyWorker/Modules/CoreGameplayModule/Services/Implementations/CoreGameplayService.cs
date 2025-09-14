using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.CoreGameplayModule.Dtos;
using F1FantasyWorker.Modules.CoreGameplayModule.Dtos.Mapper;
using F1FantasyWorker.Modules.CoreGameplayModule.Helpers;
using F1FantasyWorker.Modules.CoreGameplayModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.CoreGameplayModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Services.Implementations;

public class CoreGameplayService : ICoreGameplayService
{
     private readonly IPowerupSyncService _powerupSyncService;
     private readonly ICoreGameplayRepository _coreGameplayRepository;
     private readonly WooF1Context _context;

     public CoreGameplayService(IPowerupSyncService powerupSyncService, ICoreGameplayRepository coreGameplayRepository, WooF1Context context)
     {
          _powerupSyncService = powerupSyncService;
          _coreGameplayRepository = coreGameplayRepository;
          _context = context;
     }
    
     public async Task CalculatePointsForAllUsersInLastestFinishedRaceAsync()
     {
          using var transaction = await _context.Database.BeginTransactionAsync();

          // Get powerups
          var powerupDtos = await _powerupSyncService.GetPowerupsFromStaticResourcesAsync();
          try
          {
               var lastestFinishedRace = GetLatestFinishedRaceInCurrentSeasonWithResultAsync().Result;
          
               if(lastestFinishedRace == null || lastestFinishedRace.Calculated) return;
               
               foreach (var raceEntry in lastestFinishedRace.RaceEntries)
               {
                    int point = 0;
                    point += PointCalculationHelper.GetDriverFastestLapPoint(raceEntry.FastestLap);
                    point += PointCalculationHelper.GetDriverRacePoint(raceEntry.Position, raceEntry.Finished);
                    point += PointCalculationHelper.GetDriverQualificationPoint(raceEntry.Grid);
                    point += PointCalculationHelper.GetDriverPositionGainPoint(startPosition: raceEntry.Grid, endPosition: raceEntry.Position);
                    raceEntry.PointsGained = point;
               }
          
               await _context.Entry(lastestFinishedRace).Collection(r => r.FantasyLineups).LoadAsync();

               var driverPoints = lastestFinishedRace.RaceEntries.ToDictionary(e => e.DriverId, e => e.PointsGained);

               foreach (var lineup in lastestFinishedRace.FantasyLineups)
               {
                    await _context.Entry(lineup).Collection(l => l.Drivers).LoadAsync();

                    // Create dictionary to hold driverId and points
                    var pointFromOwnedDriversInLineUp = new Dictionary<int, int>();
                    foreach (var driverId in lineup.Drivers.Select(ld => ld.Id))
                    {
                         if (driverPoints.TryGetValue(driverId, out var points))
                         {
                              pointFromOwnedDriversInLineUp.Add(driverId, points);
                         }
                         else
                         {
                              pointFromOwnedDriversInLineUp.Add(driverId, 0);
                         }
                    }
                    // Load powerups from fantasy lineup
                    await _context.Entry(lineup).Collection(l => l.PowerupFantasyLineups).LoadAsync();
                    
                    // Get all powerups in the lineup by joining with powerups table
                    var powerUpOwned = lineup.PowerupFantasyLineups
                         .Join(powerupDtos,
                              pl => pl.PowerupId,
                              p => p.Id,
                              (pl, p) => CoreGameplayDtoMapper.MapToPowerupForPointApplicationDto(p, pl))
                         .ToList();
                    
                    int transferPointsDeducted = ApplyPowerupToDriverPointsAndCalculateTransferPointDeducted(pointFromOwnedDriversInLineUp, powerUpOwned, lineup.TransferPointsDeducted);

                    // sum up all points from drivers in lineup
                    var totalPointFromDrivers = pointFromOwnedDriversInLineUp.Values.Sum();

                    lineup.PointsGained = totalPointFromDrivers;
                    lineup.TotalAmount = totalPointFromDrivers - transferPointsDeducted;
                    // Use totalPoints as needed
               }

               lastestFinishedRace.Calculated = true;
               
               await _context.SaveChangesAsync();
               await transaction.CommitAsync();
          }
          catch (Exception ex)
          {
               await transaction.RollbackAsync();
               Console.WriteLine($"Error calculating points: {ex.Message}");

               throw;
          }
          
     }
     
     // Apply powerup effects to the points of drivers in the lineup and return the final transfer points deducted
     private int ApplyPowerupToDriverPointsAndCalculateTransferPointDeducted(Dictionary<int, int> pointFromOwnedDriversInLineUp, List<PowerupForPointApplicationDto> powerupsUsed, int transferPointsDeducted)
     {
          var finalTransferPointsDeducted = transferPointsDeducted;
          foreach (var powerupUsed in powerupsUsed)
          {
               switch (powerupUsed.Type)
               {
                    case "DRS Enabled":
                    // Double the points of the driver from the id specified in the powerupUsed.TargetDriverId
                         if (powerupUsed.DriverId == null) break;
                         if (pointFromOwnedDriversInLineUp.ContainsKey(powerupUsed.DriverId.Value))
                         {
                              pointFromOwnedDriversInLineUp[powerupUsed.DriverId.Value] *= 2;
                         }
                         break;
                    case "Free Transfer Week":
                         // Add back the points deducted for transfers
                         finalTransferPointsDeducted = 0;
                         break;
                    case "Escapist":
                         // Change every negative value in dict pointFromOwnedDriversInLineUp to 0
                         foreach (var driverId in pointFromOwnedDriversInLineUp.Keys.ToList())
                         {
                              if (pointFromOwnedDriversInLineUp[driverId] < 0)
                              {
                                   pointFromOwnedDriversInLineUp[driverId] = 0;
                              }
                         }
                         break;
                    // Add more powerup logic as needed
               }
          }
          
          return finalTransferPointsDeducted;
     } 
     
     private async Task<Race?> GetLatestFinishedRaceInCurrentSeasonWithResultAsync()
     {
          DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
          return await _context.Races
               .Where(r => r.DeadlineDate < currentDate)
               .OrderByDescending(r => r.DeadlineDate)
               .Include(r => r.RaceEntries)
               .AsTracking()
               .FirstOrDefaultAsync();
     }

     // This method is to migrate / copy fantasy lineups from the previous race to this race
     public async Task MigrateFantasyLineupsToRaceAsync(int raceToBeMigratedId)
     {
          using var transaction = await _context.Database.BeginTransactionAsync();
          
          try
          {
               // Get race to be migrated to
               Race raceToBeMigratedTo = await _context.Races.FirstOrDefaultAsync(r => r.Id == raceToBeMigratedId);
               
               if (raceToBeMigratedTo == null || raceToBeMigratedTo.Round == 1 )
               {
                    return;
               }
               
               // Get previous race
               Race previousRace = await _context.Races.FirstOrDefaultAsync(r => r.Id == raceToBeMigratedTo.Id - 1);

               if (previousRace == null)
               {
                    return;
               }
               
               // Load fantasy lineups for the 2 races
               await _context.Entry(raceToBeMigratedTo).Collection(r => r.FantasyLineups).LoadAsync();
               await _context.Entry(previousRace).Collection(r => r.FantasyLineups).LoadAsync();
               foreach (var previousfFantasyLineup in previousRace.FantasyLineups)
               {
                    var newFantasyLineup = raceToBeMigratedTo.FantasyLineups.FirstOrDefault(f => f.UserId == previousfFantasyLineup.UserId);
                    if (newFantasyLineup == null)
                    {
                         throw new Exception("New FantasyLineup not existed");
                    }
                    
                    await _context.Entry(previousfFantasyLineup).Collection(f => f.Drivers).LoadAsync();
                    await _context.Entry(newFantasyLineup).Collection(f => f.Drivers).LoadAsync();

                    // copy all connection to drivers from previous race to new
                    foreach (var driver in previousfFantasyLineup.Drivers)
                    {
                         if (newFantasyLineup.Drivers.Contains(driver))
                         {
                              continue;
                         }
                         var fantasyLineupDriver = new FantasyLineupDriver
                         {
                              FantasyLineupId = newFantasyLineup.Id, // or set the navigation property
                              DriverId = driver.Id
                         };
                         
                         await _coreGameplayRepository.AddFantasyLineupDriverAsync(fantasyLineupDriver);
                    }
               }
               
               await _context.SaveChangesAsync();
               await transaction.CommitAsync();
          }
          catch (Exception ex)
          {
               await transaction.RollbackAsync();
               Console.WriteLine($"Error migrating races: {ex.Message}");

               throw;
          }
     }
}