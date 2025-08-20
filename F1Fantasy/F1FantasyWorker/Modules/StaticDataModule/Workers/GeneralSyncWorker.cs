using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers
{
    internal class GeneralSyncWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GeneralSyncWorker> _logger;

        public GeneralSyncWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<GeneralSyncWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var _f1DataSyncService = scope.ServiceProvider.GetRequiredService<IF1DataSyncService>();
            
            var _seasonService = scope.ServiceProvider.GetRequiredService<ISeasonService>();
            var _driverService = scope.ServiceProvider.GetRequiredService<IDriverService>();
            var _constructorService = scope.ServiceProvider.GetRequiredService<IConstructorService>();
            var _circuitService = scope.ServiceProvider.GetRequiredService<ICircuitService>();
            var _nationalityService = scope.ServiceProvider.GetRequiredService<ICountryService>();
            var _raceService = scope.ServiceProvider.GetRequiredService<IRaceService>();
            var _powerupService = scope.ServiceProvider.GetRequiredService<IPowerupService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("GeneralSyncWorker running at: {time}", DateTimeOffset.Now);

                    _logger.LogInformation("Starting F1 data synchronization...");
                }
                
                // Sync powerups
                List<PowerupDto> newPowerups = await _f1DataSyncService.GetPowerupsAsync();
                foreach (var powerupDto in newPowerups)
                {
                    await _powerupService.AddPowerupAsync(powerupDto);
                }
                Console.WriteLine("Database synced with new powerups.");
                
                // Sync countries
                List<CountryDto> newCountryDtos = await _f1DataSyncService.GetCountriesAsync();
                foreach (var countryDto in newCountryDtos)
                {
                    await _nationalityService.AddCountryAsync(countryDto);
                }
                Console.WriteLine("Database synced with new countries.");

                // Sync drivers
                List<DriverApiDto> newTempDrivers = await _f1DataSyncService.GetDriversAsync();
                List<DriverDto> newDriverDtos = new List<DriverDto>();
                foreach (var tempDriver in newTempDrivers)
                {
                    newDriverDtos.Add(new DriverDto(id: null, tempDriver.GivenName, tempDriver.FamilyName, tempDriver.DateOfBirth, tempDriver.Nationality, tempDriver.DriverId, null));
                }

                foreach (var driverDto in newDriverDtos)
                {
                    await _driverService.AddDriverAsync(driverDto);
                }

                Console.WriteLine("Database synced with new drivers.");

                // Sync constructor
                List<ConstructorApiDto> newTempConstructors = await _f1DataSyncService.GetConstructorsAsync();

                List<ConstructorDto> newConstructorDtos = new List<ConstructorDto>();
                foreach (var tempConstructor in newTempConstructors)
                {
                    newConstructorDtos.Add(new ConstructorDto(id: null,tempConstructor.Name, tempConstructor.Nationality, tempConstructor.ConstructorId, null));
                }

                foreach (var constructorDto in newConstructorDtos)
                {
                    await _constructorService.AddConstructorAsync(constructorDto);
                }

                Console.WriteLine("Database synced with new constructors.");

                // Sync circuits
                List<CircuitApiDto> newTempCircuits = await _f1DataSyncService.GetCircuitsAsync();

                List<CircuitDto> newCircuitDtos = new List<CircuitDto>();
                foreach (var tempCircuit in newTempCircuits)
                {
                    newCircuitDtos.Add(new CircuitDto(id: null, tempCircuit.CircuitName, tempCircuit.CircuitId, tempCircuit.Location.Lat, tempCircuit.Location.Long, tempCircuit.Location.Locality, tempCircuit.Location.Country, null));
                }//(int circuitName, string code, decimal lattitude, decimal longttitude, string locality, string nationality, string? imgUrl)

                foreach (var circuitDto in newCircuitDtos)
                {
                    await _circuitService.AddCircuitAsync(circuitDto);
                }

                Console.WriteLine("Database synced with new circuits.");
                
                // Sync seasons
                List<SeasonApiDto> newTempSeasons = await _f1DataSyncService.GetSeasonsAsync();
                
                List<SeasonDto> newSeasonDtos = new List<SeasonDto>();
                foreach (var tempSeason in newTempSeasons)
                {
                    newSeasonDtos.Add(new SeasonDto(id: null, tempSeason.Season, isActive:false));
                }
                
                foreach (var seasonDto in newSeasonDtos)
                {
                    await _seasonService.AddSeasonAsync(seasonDto);
                }
                
                Console.WriteLine("Database synced with new seasons.");
                
                // Sync races
                List<RaceApiDto> newTempRaces = await _f1DataSyncService.GetRacesAsync();
                List<RaceDto> newRaceDtos = new List<RaceDto>();
                foreach (var tempRace in newTempRaces)
                {
                    newRaceDtos.Add(new RaceDto(id: null, tempRace.Date, tempRace.Date.AddDays(-2), false, seasonId: null, circuitId: null, tempRace.Circuit.CircuitId));
                }

                foreach (var raceDto in newRaceDtos)
                {
                    await _raceService.AddRaceAsync(raceDto);
                }

                Console.WriteLine("Database synced with new races.");
                
                //wait 2 days
                _logger.LogInformation("GeneralSyncWorker completed at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("Next synchronization will occur at: {time}", DateTimeOffset.Now.AddDays(2));
                await Task.Delay(TimeSpan.FromDays(2), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Sync Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}