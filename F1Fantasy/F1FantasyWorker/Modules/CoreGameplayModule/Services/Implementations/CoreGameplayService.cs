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
          var budget = configuration.GetSection("CoreGameplaySettings:FantasyTeamSettings:LineupBudget").Get<int>();
          var priceKFactor = configuration.GetSection("CoreGameplaySettings:FantasyTeamSettings:PriceKFactor").Get<double>();
          var minPrice = configuration.GetSection("CoreGameplaySettings:FantasyTeamSettings:MinPrice").Get<int>();
          var maxPrice = configuration.GetSection("CoreGameplaySettings:FantasyTeamSettings:MaxPrice").Get<int>();

          await using var transaction = await context.Database.BeginTransactionAsync();

          // Get powerups
          var powerupDtos = await powerupSyncService.GetPowerupsFromStaticResourcesAsync();
          try
          {
               var lastestFinishedRace = GetLatestFinishedRaceInCurrentSeasonWithResultAsync().Result;
               if(lastestFinishedRace == null || lastestFinishedRace.Calculated) return null;
               
               // Ensure its at least a day currently after the race date
               var raceCalculationDeadline = lastestFinishedRace.RaceDate.AddDays(1);
               if(raceCalculationDeadline >= DateOnly.FromDateTime(DateTime.UtcNow)) return null;
                    
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

               /*
                For constructors
                    Both drivers in top 3: 10 points
                    One driver in top 3: 5 points
                    Both drivers in top 10: 3 points
                    One driver in top 10: 1 point
                    Neither in top 10: -1 point
                */
               // Get points for each constructor in the race
               var constructorPoints = new Dictionary<int, int>();
               var constructorIdsInRace = lastestFinishedRace.RaceEntries.Select(e => e.ConstructorId).Distinct().ToList();
               // create a dictionary with constructorId as key and 0 as value
               foreach (var constructorId in constructorIdsInRace)
               {
                    constructorPoints[constructorId] = 0;
                    var raceEntriesForConstructorPointCalculation = lastestFinishedRace.RaceEntries
                         .Where(e => e.ConstructorId == constructorId)
                         .ToList();
                    
                    var positions = raceEntriesForConstructorPointCalculation.Select(d => d.Position).ToList();

                    if (positions.Count == 2)
                    {
                         bool bothTop3 = positions.All(p => p > 0 && p <= 3);
                         bool oneTop3 = positions.Count(p => p > 0 && p <= 3) == 1;
                         bool bothTop10 = positions.All(p => p > 0 && p <= 10);
                         bool oneTop10 = positions.Count(p => p > 0 && p <= 10) == 1;

                         if (bothTop3)
                              constructorPoints[constructorId] += 10;
                         else if (oneTop3)
                              constructorPoints[constructorId] += 5;
                         else if (bothTop10)
                              constructorPoints[constructorId] += 3;
                         else if (oneTop10)
                              constructorPoints[constructorId] += 1;
                         else
                              constructorPoints[constructorId] += -1;
                    }
                    else
                    {
                         // Handle missing drivers if needed
                         constructorPoints[constructorId] = -1;
                    }
               }
                    
               foreach (var lineup in lastestFinishedRace.FantasyLineups)
               {
                    await context.Entry(lineup).Collection(l => l.FantasyLineupDrivers).LoadAsync();
                    // Load constructors from fantasy lineup
                    await context.Entry(lineup).Collection(l => l.Constructors).LoadAsync();
                    
                    // If total price of drivers and constructors exceed the budget, set points to 0
                    var totalPrice = lineup.FantasyLineupDrivers.Sum(d => d.Driver.Price) + lineup.Constructors.Sum(c => c.Price);
                    if (totalPrice > budget)
                    {
                         lineup.PointsGained = 0;
                         lineup.TotalAmount = 0;
                         continue;
                    }
                    
                    // Create dictionary to hold driverId and points
                    var pointFromOwnedDriversInLineUp = new Dictionary<int, int>();
                    foreach (var driver in lineup.FantasyLineupDrivers)
                    {
                         if (driverPoints.TryGetValue(driver.DriverId, out var points))
                         {
                              // x2 points if the driver is captain
                              if (driver.IsCaptain)
                              {
                                   pointFromOwnedDriversInLineUp.Add(driver.DriverId, points*2);
                              }
                              else
                              {
                                   pointFromOwnedDriversInLineUp.Add(driver.DriverId, points);
                              }
                         }
                         else
                         {
                              pointFromOwnedDriversInLineUp.Add(driver.DriverId, 0);
                         }
                    }
                    
                    
                    int totalPointsFromConstructor = 0;

                    foreach (var constructor in lineup.Constructors)
                    {
                         if (constructorPoints.TryGetValue(constructor.Id, out var points))
                         {
                              totalPointsFromConstructor += points;
                         }
                    }
                    
                    // Load powerups from fantasy lineup
                    await context.Entry(lineup).Collection(l => l.Powerups).LoadAsync();
                    
                    // Get all powerups in the lineup by joining with powerups table
                    var powerUpOwned = lineup.Powerups
                         .Join(powerupDtos,
                              pl => pl.Id,
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
               await UpdateDriverPricesAsync(lastestFinishedRace, k: priceKFactor, minPrice: minPrice, maxPrice: maxPrice);
               await UpdateConstructorPricesAfterRaceAsync(lastestFinishedRace, k: priceKFactor, minPrice: minPrice, maxPrice: maxPrice);
               
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
                    // Double the points of the driver with the highest points in the lineup
                         if (pointFromOwnedDriversInLineUp.Count == 0) break;
                         var maxPoint = pointFromOwnedDriversInLineUp.Values.Max();
                         var driverIdWithMaxPoint = pointFromOwnedDriversInLineUp.FirstOrDefault(x => x.Value == maxPoint).Key;
                         pointFromOwnedDriversInLineUp[driverIdWithMaxPoint] *= 2;
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
          DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
          return await context.Races
               .Where(r => r.DeadlineDate < currentDate)
               .OrderByDescending(r => r.DeadlineDate)
               .Include(r => r.Season)
               .Include(r => r.RaceEntries)
               .ThenInclude(re => re.Driver)
               .Include(r => r.RaceEntries)
               .ThenInclude(re => re.Constructor)
               .AsTracking()
               .FirstOrDefaultAsync();
     }

     // This method is to migrate / copy fantasy lineups from the previous race to this race
     public async Task MigrateFantasyLineupsToNextRaceAsync(Race previousRace)
     {
          await using var transaction = await context.Database.BeginTransactionAsync();
          
          try
          {
               // Get race to be migrated to
               var raceToBeMigratedTo = await context.Races.FirstOrDefaultAsync(r => r.SeasonId == previousRace.SeasonId && r.Round == previousRace.Round + 1);
               
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
                    
                    await context.Entry(previousFantasyLineup).Collection(f => f.FantasyLineupDrivers).LoadAsync();
                    await context.Entry(newFantasyLineup).Collection(f => f.FantasyLineupDrivers).LoadAsync();
                    await context.Entry(previousFantasyLineup).Collection(f => f.Constructors).LoadAsync();
                    await context.Entry(newFantasyLineup).Collection(f => f.Constructors).LoadAsync();
                    
                    // Clear all drivers and constructors from new lineup first
                    newFantasyLineup.FantasyLineupDrivers.Clear();
                    newFantasyLineup.Constructors.Clear();
                    
                    // copy all connection to drivers from previous race to new
                    foreach (var fantasyLineupDriver in previousFantasyLineup.FantasyLineupDrivers)
                    {
                         if (newFantasyLineup.FantasyLineupDrivers.Any(fld => fld.DriverId == fantasyLineupDriver.DriverId))
                         {
                              continue;
                         }
                         
                         await coreGameplayRepository.AddFantasyLineupDriverAsync(newFantasyLineup, fantasyLineupDriver.Driver);
                    }
                    
                    // copy all connection to constructors from previous race to new
                    foreach (var constructor in previousFantasyLineup.Constructors)
                    {
                         if (newFantasyLineup.Constructors.Any(c => c.Id == constructor.Id))
                         {
                              continue;
                         }
                         
                         newFantasyLineup.Constructors.Add(constructor);
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
     
     public async Task UpdateDriverPricesAsync(Race race, double k = 0.2, int minPrice = 5, int maxPrice = 100)
     {
          var drivers = race.RaceEntries.Select(re => re.Driver).Distinct().ToList();
          // Load Season for race
          foreach (var driver in drivers)
          {
               var raceResults = await context.RaceEntries
                    .Where(r => r.DriverId == driver.Id && r.Race.SeasonId == race.SeasonId)
                    .OrderBy(r => r.Race.RaceDate)
                    .ToListAsync();

               if (raceResults.Count < 2)
                    continue;

               var lastResult = raceResults.Last();
               var previousResults = raceResults.Take(raceResults.Count - 1).ToList();

               double sRace = lastResult.PointsGained;
               double sAvg = previousResults.Average(r => r.PointsGained);

               double priceRange = maxPrice - minPrice;
               double priceFactor = 1.0 - ((driver.Price - minPrice) / priceRange);
               priceFactor = Math.Max(0.1, priceFactor); // Prevents priceFactor from going to zero

               double delta = k * (sRace - sAvg) * priceFactor;
               double newPrice = driver.Price + delta;
               newPrice = Math.Max(minPrice, Math.Min(maxPrice, Math.Round(newPrice)));

               if (driver.Price != (int)newPrice)
               {
                    driver.Price = (int)newPrice;
                    context.Drivers.Update(driver);
               }
          }
          await context.SaveChangesAsync();
     }
     
     // Calculate new prices for constructors based on their drivers' performance
     public async Task UpdateConstructorPricesAfterRaceAsync(Race race, double k = 0.2, int minPrice = 5, int maxPrice = 100)
     {
          var constructors =  race.RaceEntries.Select(re => re.Constructor).Distinct().ToList();
          foreach (var constructor in constructors)
          {
               // Get all race results for this constructor up to and including this race
               var raceResults = await context.RaceEntries
                    .Where(r => r.ConstructorId == constructor.Id && r.Race.SeasonId == race.SeasonId)
                    .OrderBy(r => r.Race.RaceDate)
                    .ToListAsync();
               
               // Combine race results where has same raceId (i.e. both drivers in same race)
               raceResults = raceResults
                    .GroupBy(r => r.RaceId)
                    .Select(g =>
                    {
                         var combined = g.First();
                         combined.PointsGained = g.Sum(x => x.PointsGained);
                         return combined;
                    })
                    .ToList();
               
               if (raceResults.Count < 2)
                    continue; // Not enough data to adjust

               var lastResult = raceResults.Last();
               var previousResults = raceResults.Take(raceResults.Count - 1).ToList();

               double sRace = lastResult.PointsGained;
               double sAvg = previousResults.Average(r => r.PointsGained);

               double priceRange = maxPrice - minPrice;
               double priceFactor = 1.0 - ((constructor.Price - minPrice) / priceRange);
               priceFactor = Math.Max(0.1, priceFactor); // Prevents priceFactor from going to zero

               double delta = k * (sRace - sAvg) * priceFactor;
               double newPrice = constructor.Price + delta;
               newPrice = Math.Max(minPrice, Math.Min(maxPrice, Math.Round(newPrice)));

               if (constructor.Price != (int)newPrice)
               {
                    constructor.Price = (int)newPrice;
                    context.Constructors.Update(constructor);
               }
          }
          await context.SaveChangesAsync();
     }
}