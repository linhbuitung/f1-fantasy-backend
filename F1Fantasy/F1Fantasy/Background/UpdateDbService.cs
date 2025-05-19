using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Infrastructure.ExternalServices;
using F1Fantasy.Infrastructure.ExternalServices.TempModels;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Services.Implementations;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using System.IO.Compression;
using System.Net;

namespace F1Fantasy.Background
{
    public class UpdateDbService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IServiceProvider _serviceProvider;

        public UpdateDbService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var zipFileUrl = _configuration["DownloadSettings:DownloadUrl"];
            var downloadPath = _configuration["DownloadSettings:DownloadLocation"];
            var fileName = _configuration["DownloadSettings:FileName"];
            var extractPath = _configuration["DownloadSettings:ExtractLocation"];

            if (fileName == null)
            {
                fileName = "f1db_csv.zip";
            }
            if (extractPath == null)
            {
                extractPath = Path.Combine(downloadPath, "Csv");
            }
            if (string.IsNullOrEmpty(zipFileUrl) || string.IsNullOrEmpty(downloadPath))
            {
                throw new InvalidOperationException("DownloadSettings:ZipFileUrl or DownloadSettings:DownloadPath is not configured in appsettings.json.");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Ensure the download directory exists
                    Directory.CreateDirectory(downloadPath);

                    var filePath = Path.Combine(downloadPath, fileName);

                    // Download the ZIP file
                    //await DownloadFileAsync(zipFileUrl, filePath, stoppingToken);

                    Console.WriteLine($"Zip downloaded successfully to {filePath}");

                    //Extract the ZIP file
                    ZipFile.ExtractToDirectory(filePath, extractPath, true);
                    Console.WriteLine($"CSV extracted successfully to {extractPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading file: {ex.Message}");
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WooF1Context>();
                    if (await context.Database.CanConnectAsync())
                    {
                        Console.WriteLine("Successfully connected to the database.");
                        try
                        {
                            UpdateDriversFromCsv(extractPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating database: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to connect to the database.");
                    }
                }

                // Wait for 3 days
                await Task.Delay(TimeSpan.FromDays(3), stoppingToken);
            }
        }

        private async Task DownloadFileAsync(string url, string filePath, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await contentStream.CopyToAsync(fileStream, cancellationToken);
        }

        private async void UpdateDriversFromCsv(string extractPath)
        {
            using (var scope = _serviceProvider.CreateScope()) // Create a new scope
            {
                var driverService = scope.ServiceProvider.GetRequiredService<IDriverService>();
                var constructorService = scope.ServiceProvider.GetRequiredService<IConstructorService>();
                var circuitService = scope.ServiceProvider.GetRequiredService<ICircuitService>();
                /* var csvFilePath = Path.Combine(extractPath, "drivers.csv");
                 if (!File.Exists(csvFilePath))
                 {
                     Console.WriteLine($"CSV file not found at {csvFilePath}");
                     return;
                 }
                 //check if header are correct order
                 //driverId,driverRef,number,code,forename,surname,dob,nationality,url

                 string header = File.ReadLines(csvFilePath).First();
                 if (!header.Equals("driverId,driverRef,number,code,forename,surname,dob,nationality,url"))
                 {
                     throw new InvalidOperationException("CSV file header is not in the correct order.");
                 }*/

                F1DataSyncService f1DataService = scope.ServiceProvider.GetRequiredService<F1DataSyncService>();

                List<DriverApiDto> newTempDrivers = await f1DataService.GetDriversAsync();

                List<DriverDto> newDriverDtos = new List<DriverDto>();
                foreach (var tempDriver in newTempDrivers)
                {
                    newDriverDtos.Add(new DriverDto(tempDriver.GivenName, tempDriver.FamilyName, tempDriver.DateOfBirth, tempDriver.Nationality, tempDriver.DriverId, null));
                }

                foreach (var driverDto in newDriverDtos)
                {
                    await driverService.AddDriverAsync(driverDto);
                }

                Console.WriteLine("Database synced with new drivers.");

                List<ConstructorApiDto> newTempConstructors = await f1DataService.GetConstructorsAsync();

                List<ConstructorDto> newConstructorDtos = new List<ConstructorDto>();
                foreach (var tempConstructor in newTempConstructors)
                {
                    newConstructorDtos.Add(new ConstructorDto(tempConstructor.Name, tempConstructor.Nationality, tempConstructor.ConstructorId, null));
                }

                foreach (var constructorDto in newConstructorDtos)
                {
                    await constructorService.AddConstructorAsync(constructorDto);
                }

                Console.WriteLine("Database synced with new constructors.");

                List<CircuitApiDto> newTempCircuits = await f1DataService.GetCircuitsAsync();

                List<CircuitDto> newCircuitDtos = new List<CircuitDto>();
                foreach (var tempCircuit in newTempCircuits)
                {
                    newCircuitDtos.Add(new CircuitDto(tempCircuit.CircuitName, tempCircuit.CircuitId, tempCircuit.Location.Lat, tempCircuit.Location.Long, tempCircuit.Location.Locality, tempCircuit.Location.Country, null));
                }//(int circuitName, string code, decimal lattitude, decimal longttitude, string locality, string nationality, string? imgUrl)

                foreach (var circuitDto in newCircuitDtos)
                {
                    await circuitService.AddCircuitAsync(circuitDto);
                }

                Console.WriteLine("Database synced with new circuits.");
            }
        }

        private void UpdateCircuitsFromCsv(string extractPath)
        {
            var csvFilePath = Path.Combine(extractPath, "circuits.csv");
            if (!File.Exists(csvFilePath))
            {
                Console.WriteLine($"CSV file not found at {csvFilePath}");
                return;
            }
            //check if header are correct order
            //dcircuitId,circuitRef,name,location,country,lat,lng,alt,url

            string header = File.ReadLines(csvFilePath).First();
            if (!header.Equals("circuitId,circuitRef,name,location,country,lat,lng,alt,url"))
            {
                throw new InvalidOperationException("CSV file header is not in the correct order.");
            }

            //List<CircuitApiDto> newTempCircuits = File.ReadAllLines(csvFilePath)
            //                              .Skip(1)
            //                              .Select(v => CircuitApiDto.FromCsvLine(v))
            //                              .ToList();
            //Console.WriteLine(newTempCircuits[20]);
            /*using (var scope = new ServiceScopeFactory().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WooF1Context>();

                foreach (var driver in newDrivers)
                {
                    if (!context.Drivers.Any(d => d.GivenName == driver.GivenName && d.FamilyName == driver.FamilyName))
                    {
                        context.Drivers.Add(driver);
                    }
                }

                context.SaveChanges();
            }*/

            Console.WriteLine("Database updated with new drivers.");
        }
    }
}