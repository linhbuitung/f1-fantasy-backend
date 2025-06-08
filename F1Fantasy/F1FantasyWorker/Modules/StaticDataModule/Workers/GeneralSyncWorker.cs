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
        private readonly ILogger<GeneralSyncWorker> _logger;
        private readonly IF1DataSyncService _f1DataSyncService;
        private readonly IDriverService _driverService;
        private readonly IConstructorService _constructorService;
        private readonly ICircuitService _circuitService;

        public GeneralSyncWorker(
            ILogger<GeneralSyncWorker> logger,
            IF1DataSyncService f1DataSyncService,
            IDriverService driverService,
            IConstructorService constructorService,
            ICircuitService circuitService)
        {
            _logger = logger;
            _f1DataSyncService = f1DataSyncService;
            _driverService = driverService;
            _constructorService = constructorService;
            _circuitService = circuitService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("GeneralSyncWorker running at: {time}", DateTimeOffset.Now);

                    _logger.LogInformation("Starting F1 data synchronization...");
                    //sync drivers
                    List<DriverApiDto> newTempDrivers = await _f1DataSyncService.GetDriversAsync();
                    List<DriverDto> newDriverDtos = new List<DriverDto>();
                    foreach (var tempDriver in newTempDrivers)
                    {
                        newDriverDtos.Add(new DriverDto(tempDriver.GivenName, tempDriver.FamilyName, tempDriver.DateOfBirth, tempDriver.Nationality, tempDriver.DriverId, null));
                    }

                    foreach (var driverDto in newDriverDtos)
                    {
                        await _driverService.AddDriverAsync(driverDto);
                    }
                }

                Console.WriteLine("Database synced with new drivers.");

                List<ConstructorApiDto> newTempConstructors = await _f1DataSyncService.GetConstructorsAsync();

                List<ConstructorDto> newConstructorDtos = new List<ConstructorDto>();
                foreach (var tempConstructor in newTempConstructors)
                {
                    newConstructorDtos.Add(new ConstructorDto(tempConstructor.Name, tempConstructor.Nationality, tempConstructor.ConstructorId, null));
                }

                foreach (var constructorDto in newConstructorDtos)
                {
                    await _constructorService.AddConstructorAsync(constructorDto);
                }

                Console.WriteLine("Database synced with new constructors.");

                List<CircuitApiDto> newTempCircuits = await _f1DataSyncService.GetCircuitsAsync();

                List<CircuitDto> newCircuitDtos = new List<CircuitDto>();
                foreach (var tempCircuit in newTempCircuits)
                {
                    newCircuitDtos.Add(new CircuitDto(tempCircuit.CircuitName, tempCircuit.CircuitId, tempCircuit.Location.Lat, tempCircuit.Location.Long, tempCircuit.Location.Locality, tempCircuit.Location.Country, null));
                }//(int circuitName, string code, decimal lattitude, decimal longttitude, string locality, string nationality, string? imgUrl)

                foreach (var circuitDto in newCircuitDtos)
                {
                    await _circuitService.AddCircuitAsync(circuitDto);
                }

                Console.WriteLine("Database synced with new circuits.");
                //wait 2 days
                await Task.Delay(TimeSpan.FromDays(2), stoppingToken);
            }
        }
    }
}