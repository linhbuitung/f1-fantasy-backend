using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1FantasyWorker.Modules.CoreGameplayModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers
{
    internal class GeneralSyncWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<GeneralSyncWorker> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            #region AddScopedServices

            using var scope = scopeFactory.CreateScope();
            
            var seasonService = scope.ServiceProvider.GetRequiredService<ISeasonService>();
            var driverService = scope.ServiceProvider.GetRequiredService<IDriverService>();
            var constructorService = scope.ServiceProvider.GetRequiredService<IConstructorService>();
            var circuitService = scope.ServiceProvider.GetRequiredService<ICircuitService>();
            var countryService = scope.ServiceProvider.GetRequiredService<ICountryService>();
            var raceService = scope.ServiceProvider.GetRequiredService<IRaceService>();
            var powerupService = scope.ServiceProvider.GetRequiredService<IPowerupService>();
            var raceEntryService = scope.ServiceProvider.GetRequiredService<IRaceEntryService>();
            var fantasyLineupService = scope.ServiceProvider.GetRequiredService<IFantasyLineupService>();
            var pickableItemService = scope.ServiceProvider.GetRequiredService<IPickableItemService>();
            
            var seasonSyncService = scope.ServiceProvider.GetRequiredService<ISeasonSyncService>();
            var driveSyncService = scope.ServiceProvider.GetRequiredService<IDriverSyncService>();
            var constructorSyncService = scope.ServiceProvider.GetRequiredService<IConstructorSyncService>();
            var circuitSyncService = scope.ServiceProvider.GetRequiredService<ICircuitSyncService>();
            var countrySyncService = scope.ServiceProvider.GetRequiredService<ICountrySyncService>();
            var raceSyncService = scope.ServiceProvider.GetRequiredService<IRaceSyncService>();
            var powerupSyncService = scope.ServiceProvider.GetRequiredService<IPowerupSyncService>();
            var raceEntrySyncService = scope.ServiceProvider.GetRequiredService<IRaceEntrySyncService>();
            
            var coreGameplayService = scope.ServiceProvider.GetRequiredService<ICoreGameplayService>();
            #endregion
            

            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("GeneralSyncWorker running at: {time}", DateTimeOffset.Now);

                    logger.LogInformation("Starting F1 data synchronization...");
                }
                logger.LogInformation("Start syncing pickable items");
                var pickableItemId = await pickableItemService.GetPickableItemAsync();
                if (pickableItemId == null)
                {
                    await pickableItemService.AddPickableItemAsync();
                }
                logger.LogInformation("Database synced with 1 edition of pickable.");

                // Sync powerups
                logger.LogInformation("Start syncing powerups");
                
                List<PowerupDto> newPowerups = await powerupSyncService.GetPowerupsFromStaticResourcesAsync();
                foreach (var powerupDto in newPowerups)
                {
                    await powerupService.AddPowerupAsync(powerupDto);
                }
                logger.LogInformation("Database synced with new powerups.");
                
                // Sync countries
                logger.LogInformation("Start syncing countries");
                
                List<CountryDto> newCountryDtos = await countrySyncService.GetCountriesFromStaticResourcesAsync();
                foreach (var countryDto in newCountryDtos)
                {
                    await countryService.AddCountryAsync(countryDto);
                }
                logger.LogInformation("Database synced with new countries.");

                // Sync drivers
                logger.LogInformation("Start syncing drivers");
                
                List<DriverApiDto> newTempDrivers = await driveSyncService.GetDriversFromApiAsync();
                List<DriverDto> newDriverDtos = new List<DriverDto>();
                foreach (var tempDriver in newTempDrivers)
                {
                    newDriverDtos.Add(new DriverDto(id: null, tempDriver.GivenName, tempDriver.FamilyName, tempDriver.DateOfBirth, tempDriver.Nationality, tempDriver.DriverId, 0, null));
                }

                foreach (var driverDto in newDriverDtos)
                {
                    await driverService.AddDriverAsync(driverDto);
                }

                logger.LogInformation("Database synced with new drivers.");

                // Sync constructor
                logger.LogInformation("Start syncing constructors");
                
                List<ConstructorApiDto> newTempConstructors = await constructorSyncService.GetConstructorsFromApiAsync();

                List<ConstructorDto> newConstructorDtos = new List<ConstructorDto>();
                foreach (var tempConstructor in newTempConstructors)
                {
                    newConstructorDtos.Add(new ConstructorDto(id: null,tempConstructor.Name, tempConstructor.Nationality, tempConstructor.ConstructorId, null));
                }

                foreach (var constructorDto in newConstructorDtos)
                {
                    await constructorService.AddConstructorAsync(constructorDto);
                }

                logger.LogInformation("Database synced with new constructors.");

                // Sync circuits
                logger.LogInformation("Start syncing circuits");
                
                List<CircuitApiDto> newTempCircuits = await circuitSyncService.GetCircuitsFromApiAsync();

                List<CircuitDto> newCircuitDtos = new List<CircuitDto>();
                foreach (var tempCircuit in newTempCircuits)
                {
                    newCircuitDtos.Add(new CircuitDto(id: null, tempCircuit.CircuitName, tempCircuit.CircuitId, tempCircuit.Location.Lat, tempCircuit.Location.Long, tempCircuit.Location.Locality, tempCircuit.Location.Country, null));
                }
                
                foreach (var circuitDto in newCircuitDtos)
                {
                    await circuitService.AddCircuitAsync(circuitDto);
                }

                logger.LogInformation("Database synced with new circuits.");
                
                // Sync seasons
                logger.LogInformation("Start syncing seasons");
                
                List<SeasonApiDto> newTempSeasons = await seasonSyncService.GetSeasonsFromApiAsync();
                
                List<SeasonDto> newSeasonDtos = new List<SeasonDto>();
                foreach (var tempSeason in newTempSeasons)
                {
                    newSeasonDtos.Add(new SeasonDto(id: null, tempSeason.Season, isActive:false));
                }
                
                foreach (var seasonDto in newSeasonDtos)
                {
                    await seasonService.AddSeasonAsync(seasonDto);
                }
                
                logger.LogInformation("Database synced with new seasons.");
                
                // Sync races and add fantasy lineups for all users for new races
                logger.LogInformation("Start syncing races");
                
                List<RaceApiDto> newTempRaces = await raceSyncService.GetRacesFromApiAsync();
                List<RaceDto> newRaceDtos = new List<RaceDto>();
                foreach (var tempRace in newTempRaces)
                {
                    newRaceDtos.Add(new RaceDto(id: null, tempRace.Date, tempRace.Round, tempRace.Date.AddDays(-2), false, seasonId: null, circuitId: null, tempRace.Circuit.CircuitId));
                }

                foreach (var raceDto in newRaceDtos)
                {
                    await raceService.AddRaceAsync(raceDto);
                }

                logger.LogInformation("Database synced with new races");
                
                // Add fantasy lineups for all users for current season
                logger.LogInformation("Start adding fantasy lineups for all users in current season");
                await fantasyLineupService.AddFantasyLineupForAllUsersInASeasonAsync(DateTime.Now.Year);
                logger.LogInformation("Database synced with new fantasy lineups");
                
                // Sync race entries
                logger.LogInformation("Start syncing race entries for the current year from API");
                
                List<RaceEntryApiDto> newTempRaceEntries = await raceEntrySyncService.GetRaceEntriesForCurrentYearFromApiAsync();
                List<RaceEntryDto> newRaceEntryDtos = new List<RaceEntryDto>();
                string pattern = @"\+.* Lap.*";
                foreach (var tempRaceEntry in newTempRaceEntries)
                {
                    // match the pattern or is equal to "Finished"
                    bool finished = tempRaceEntry.Status.Equals("Finished") || tempRaceEntry.Status.Equals("Lapped") ||
                                    System.Text.RegularExpressions.Regex.IsMatch(tempRaceEntry.Status, pattern);
                   
                    newRaceEntryDtos.Add(new RaceEntryDto(id: null, 
                        tempRaceEntry.Position, 
                        tempRaceEntry.Grid,
                        tempRaceEntry.FastestLap?.Rank,
                        0,
                        null,
                        tempRaceEntry.Driver.DriverId,
                        null,
                        tempRaceEntry.RaceDate,
                        null,
                        tempRaceEntry.Constructor.ConstructorId,
                        null,
                        finished
                        ));
                }
                
                foreach (var raceEntry in newRaceEntryDtos)
                {
                    await raceEntryService.AddRaceEntryAsync(raceEntry);
                }

                /*List<RaceEntryDto> newRaceEntryDtos =  await raceEntrySyncService.GetStaticRaceEntriesAsync();
                
                foreach (var raceEntry in newRaceEntryDtos)
                {
                    await raceEntryService.AddRaceEntryAsync(raceEntry);
                }*/
                logger.LogInformation("Database synced with static race entries.");
                
                // Calculate points for all users in latest race
                logger.LogInformation("Start calculating point gained for all users in latest");
                var calculatedRace = await coreGameplayService.CalculatePointsForAllUsersInLastestFinishedRaceAsync();
                logger.LogInformation("Database calculated point gained for all users in latest race.");
                
                // Migrate fantasy lineups to next race
                if (calculatedRace != null)
                {
                    logger.LogInformation("Start migrating fantasy lineups to next race");
                    await coreGameplayService.MigrateFantasyLineupsToNextRaceAsync(calculatedRace);
                    logger.LogInformation("Migrated fantasy lineups to next race");
                }

                //wait 2 days
                logger.LogInformation("GeneralSyncWorker completed at: {time}", DateTimeOffset.Now);
                logger.LogInformation("Next synchronization will occur at: {time}", DateTimeOffset.Now.AddDays(2));
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Sync Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}