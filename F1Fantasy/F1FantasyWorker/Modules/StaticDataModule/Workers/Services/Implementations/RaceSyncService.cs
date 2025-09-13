using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;
using Newtonsoft.Json;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;

public class RaceSyncService : IRaceSyncService
{
    private readonly WorkerConfigurationService _workerConfig;
    private readonly HttpClient _httpClient;
    private readonly IServiceScopeFactory _scopeFactory;

    public RaceSyncService(HttpClient httpClient, IServiceScopeFactory scopeFactory, WorkerConfigurationService workerConfig)
    {
        _scopeFactory = scopeFactory;
        _httpClient = httpClient;
        _workerConfig = workerConfig;
    }
    public class RaceTableDto
    {
        public List<RaceApiDto> Races { get; set; }
    }
    
    public class MRRaceDataDto
    {
        public int Total { get; set; }
        public RaceTableDto RaceTable { get; set; }
    }
    public class RaceApiResponseDto
    {
        public MRRaceDataDto MRData { get; set; }
    }
    
    public async Task<List<RaceApiDto>> GetRacesFromApiAsync()
    {
        int limit = _workerConfig.SyncRequestLimit;
        int offset = 0;
        string queryParams;
        string apiUrl;
        
        List<RaceApiDto> tempRaces = new List<RaceApiDto>();

        bool condition = true;
        while (condition)
        {
            await Task.Delay(_workerConfig.DelayBetweenRequests);

            queryParams = $"limit={limit}&offset={offset}";
            apiUrl = $"https://api.jolpi.ca/ergast/f1/races/?{queryParams}";

            var response = await _httpClient.GetStringAsync(apiUrl);
            if (string.IsNullOrEmpty(response))
            {
                break;
            }

            RaceApiResponseDto apiResponse = JsonConvert.DeserializeObject<RaceApiResponseDto>(response);
            if (apiResponse == null || apiResponse.MRData.RaceTable.Races.Count < 100)
            {
                condition = false;
            }

            if (offset == 0 && apiResponse != null)
            {
                using var scope = _scopeFactory.CreateScope();
                var raceService = scope.ServiceProvider.GetRequiredService<IRaceService>();
                int currentRacesCount = await raceService.GetRacesCountAsync();
                if (apiResponse.MRData.Total == currentRacesCount)
                {
                    return [];
                }
                offset = currentRacesCount;
            }
            
            
            Console.WriteLine($"offset: {offset} - Races count: {apiResponse.MRData.RaceTable.Races.Count}");
            tempRaces.AddRange(apiResponse.MRData.RaceTable.Races);

            offset += limit;
        }

        return tempRaces;
    }
}