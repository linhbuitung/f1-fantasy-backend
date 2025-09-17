using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;
using Newtonsoft.Json;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;

public class DriverSyncService(
    HttpClient httpClient,
    IServiceScopeFactory scopeFactory,
    WorkerConfigurationService workerConfig)
    : IDriverSyncService
{
    public class DriverTableDto
    {
        public List<DriverApiDto> Drivers { get; set; }
    }

    public class MRDriverDataDto
    {
        public int Total { get; set; }
        public DriverTableDto DriverTable { get; set; }
    }

    public class DriverApiResponseDto
    {
        public MRDriverDataDto MRData { get; set; }
    }

    public async Task<List<DriverApiDto>> GetDriversFromApiAsync()
    {
        int limit = workerConfig.SyncRequestLimit;
        int offset = 0;
        string queryParams;
        string apiUrl;
        
        List<DriverApiDto> tempDrivers = new List<DriverApiDto>();

        bool condition = true;
        while (condition)
        {
            await Task.Delay(workerConfig.DelayBetweenRequests);

            queryParams = $"limit={limit}&offset={offset}";
            apiUrl = $"https://api.jolpi.ca/ergast/f1/drivers/?{queryParams}";

            var response = await httpClient.GetStringAsync(apiUrl);
            if (string.IsNullOrEmpty(response))
            {
                break;
            }

            DriverApiResponseDto apiResponse = JsonConvert.DeserializeObject<DriverApiResponseDto>(response);
            
            if (apiResponse == null || apiResponse.MRData.DriverTable.Drivers.Count < 100)
            {
                condition = false;
            }
            
            Console.WriteLine($"offset: {offset} - Drivers count: {apiResponse.MRData.DriverTable.Drivers.Count}");
            tempDrivers.AddRange(apiResponse.MRData.DriverTable.Drivers);

            if (offset == 0 && apiResponse != null)
            {
                using var scope = scopeFactory.CreateScope();
                var driverService = scope.ServiceProvider.GetRequiredService<IDriverService>();
                int currentDriversCount = await driverService.GetDriversCountAsync();
                if (apiResponse.MRData.Total == currentDriversCount)
                {
                    return [];
                }
                if (currentDriversCount == 0)
                {
                    offset += limit;
                }
                else
                {
                    offset = currentDriversCount;
                }
            }
            else
            {
                offset += limit;
            }
        }

        return tempDrivers;
    }
}