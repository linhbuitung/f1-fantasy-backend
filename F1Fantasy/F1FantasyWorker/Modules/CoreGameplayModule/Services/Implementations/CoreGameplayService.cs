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

public class CoreGameplayService(
     IPowerupSyncService powerupSyncService,
     ICoreGameplayRepository coreGameplayRepository,
     IConfiguration configuration,
     WooF1Context context)
     : ICoreGameplayService
{
     public async Task<Race?> CalculatePointsForAllUsersInLastestFinishedRaceAsync()
     {
          using var transaction = await context.Database.BeginTransactionAsync();

          // Get powerups
          var powerupDtos = await powerupSyncService.GetPowerupsFromStaticResourcesAsync();
          try
          {
               var lastestFinishedRace = GetLatestFinishedRaceInCurrentSeasonWithResultAsync().Result;
               
               if(lastestFinishedRace == null || lastestFinishedRace.Calculated) return null;
               
               // Ensure its at least a day currently after the race date
               var raceCalculationDeadline = lastestFinishedRace.RaceDate.AddDays(1);
               if(raceCalculationDeadline >= DateOnly.FromDateTime(DateTime.Now)) return null;
                    
               foreach (var raceEntry in lastestFinishedRace.RaceEntries)
               {
                    int point = 0;
                    point += PointCalculationHelper.GetDriverFastestLapPoint(raceEntry.FastestLap);
                    point += PointCalculationHelper.GetDriverRacePoint(raceEntry.Position, raceEntry.Finished);
                    point += PointCalculationHelper.GetDriverQualificationPoint(raceEntry.Grid);
                    point += PointCalculationHelper.GetDriverPositionGainPoint(startPosition: raceEntry.Grid, endPosition: raceEntry.Position);
                    raceEntry.PointsGained = point;
               }
          
               await context.Entry(lastestFinishedRace).Collection(r => r.FantasyLineups).LoadAsync();

               var driverPoints = lastestFinishedRace.RaceEntries.ToDictionary(e => e.DriverId, e => e.PointsGained);

               foreach (var lineup in lastestFinishedRace.FantasyLineups)
               {
                    await context.Entry(lineup).Collection(l => l.DriversNavigation).LoadAsync();

                    // Create dictionary to hold driverId and points
                    var pointFromOwnedDriversInLineUp = new Dictionary<int, int>();
                    foreach (var driverId in lineup.DriversNavigation.Select(ld => ld.Id))
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
                    
                    // Load constructors from fantasy lineup
                    await context.Entry(lineup).Collection(l => l.ConstructorsNavigation).LoadAsync();

                    /*
                         Both drivers in top 3: 10 points
                         One driver in top 3: 5 points
                         Both drivers in top 10: 3 points
                         One driver in top 10: 1 point
                         Neither in top 10: -1 point
                     */
                    int totalPointsFromConstructor = 0;

                    foreach (var constructor in lineup.ConstructorsNavigation)
                    {
                         // Get all drivers for this constructor in the race
                         var raceEntriesForConstructorPointCalculation = lastestFinishedRace.RaceEntries
                              .Where(e => e.ConstructorId == constructor.Id)
                              .ToList();

                         // Get their positions
                         var positions = raceEntriesForConstructorPointCalculation.Select(d => d.Position).ToList();


                         if (positions.Count == 2)
                         {
                              bool bothTop3 = positions.All(p => p > 0 && p <= 3);
                              bool oneTop3 = positions.Count(p => p > 0 && p <= 3) == 1;
                              bool bothTop10 = positions.All(p => p > 0 && p <= 10);
                              bool oneTop10 = positions.Count(p => p > 0 && p <= 10) == 1;

                              if (bothTop3)
                                   totalPointsFromConstructor += 10;
                              else if (oneTop3)
                                   totalPointsFromConstructor += 5;
                              else if (bothTop10)
                                   totalPointsFromConstructor += 3;
                              else if (oneTop10)
                                   totalPointsFromConstructor += 1;
                              else
                                   totalPointsFromConstructor += -1;
                         }
                         else
                         {
                              // Handle missing drivers if needed
                              totalPointsFromConstructor = -1;
                         }
                    }
                    
                    // Load powerups from fantasy lineup
                    await context.Entry(lineup).Collection(l => l.PowerupFantasyLineups).LoadAsync();
                    
                    // Get all powerups in the lineup by joining with powerups table
                    var powerUpOwned = lineup.PowerupFantasyLineups
                         .Join(powerupDtos,
                              pl => pl.PowerupId,
                              p => p.Id,
                              (pl, p) => CoreGameplayDtoMapper.MapToPowerupForPointApplicationDto(p, pl))
                         .ToList();
                    
                    int transferPointsDeducted = ApplyPowerupToDriverPointsAndCalculateTransferPointDeducted(pointFromOwnedDriversInLineUp, powerUpOwned, lineup.TransfersMade);

                    // sum up all points from drivers in lineup
                    var totalPointFromDrivers = pointFromOwnedDriversInLineUp.Values.Sum();

                    lineup.PointsGained = totalPointFromDrivers;
                    lineup.TotalAmount = totalPointFromDrivers + totalPointsFromConstructor - transferPointsDeducted;
                    // Use totalPoints as needed
               }

               lastestFinishedRace.Calculated = true;
               
               await context.SaveChangesAsync();
               await transaction.CommitAsync();
               
               return lastestFinishedRace;
          }
          catch (Exception ex)
          {
               await transaction.RollbackAsync();
               Console.WriteLine($"Error calculating points: {ex.Message}");

               throw;
          }
          
     }
     
     // Apply powerup effects to the points of drivers in the lineup and return the final transfer points deducted
     private int ApplyPowerupToDriverPointsAndCalculateTransferPointDeducted(Dictionary<int, int> pointFromOwnedDriversInLineUp, List<PowerupForPointApplicationDto> powerupsUsed, int transfersMade)
     {
          var freeChanges = configuration.GetSection("CoreGameplaySettings:FantasyTeamSettings:FreeLineupChanges").Get<int>();
          var penaltyPerExceedingItem = configuration.GetSection("CoreGameplaySettings:FantasyTeamSettings:PenaltyPerExceedingItem").Get<int>();

          var finalTransferPointsDeducted = transfersMade - freeChanges > 0 ? (transfersMade - freeChanges) * penaltyPerExceedingItem : 0;
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
          return await context.Races
               .Where(r => r.DeadlineDate < currentDate)
               .OrderByDescending(r => r.DeadlineDate)
               .Include(r => r.RaceEntries)
               .AsTracking()
               .FirstOrDefaultAsync();
     }

     // This method is to migrate / copy fantasy lineups from the previous race to this race
     public async Task MigrateFantasyLineupsToNextRaceAsync(Race previousRace)
     {
          using var transaction = await context.Database.BeginTransactionAsync();
          
          try
          {
               // Get race to be migrated to
               Race raceToBeMigratedTo = await context.Races.FirstOrDefaultAsync(r => r.SeasonId == previousRace.SeasonId && r.Round == previousRace.Round + 1);
               
               if (raceToBeMigratedTo == null || raceToBeMigratedTo.Round == 1 )
               {
                    return;
               }
               
               // Load fantasy lineups for the 2 races
               await context.Entry(raceToBeMigratedTo).Collection(r => r.FantasyLineups).LoadAsync();
               await context.Entry(previousRace).Collection(r => r.FantasyLineups).LoadAsync();
               foreach (var previousFantasyLineup in previousRace.FantasyLineups)
               {
                    var newFantasyLineup = raceToBeMigratedTo.FantasyLineups.FirstOrDefault(f => f.UserId == previousFantasyLineup.UserId);
                    if (newFantasyLineup == null)
                    {
                         throw new Exception("New FantasyLineup not existed");
                    }
                    
                    await context.Entry(previousFantasyLineup).Collection(f => f.DriversNavigation).LoadAsync();
                    await context.Entry(newFantasyLineup).Collection(f => f.DriversNavigation).LoadAsync();

                    // copy all connection to drivers from previous race to new
                    foreach (var driver in previousFantasyLineup.DriversNavigation)
                    {
                         if (newFantasyLineup.DriversNavigation.Contains(driver))
                         {
                              continue;
                         }
                         
                         await coreGameplayRepository.AddFantasyLineupDriverAsync(newFantasyLineup, driver);
                    }
                    // transfer made
                    newFantasyLineup.TransfersMade = 0;
               }
               
               await context.SaveChangesAsync();
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