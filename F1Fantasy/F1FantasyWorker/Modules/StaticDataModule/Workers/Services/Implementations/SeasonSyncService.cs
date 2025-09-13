using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;
using Newtonsoft.Json;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;

public class SeasonSyncService : ISeasonSyncService
{
    private readonly WorkerConfigurationService _workerConfig;
    private readonly HttpClient _httpClient;
    private readonly IServiceScopeFactory _scopeFactory;

    public SeasonSyncService(HttpClient httpClient, IServiceScopeFactory scopeFactory, WorkerConfigurationService workerConfig)
    {
        _scopeFactory = scopeFactory;
        _httpClient = httpClient;
        _workerConfig = workerConfig;
    }
    public class SeasonTableDto
    {
        public List<SeasonApiDto> Seasons { get; set; }
    }

    public class MRSeasonDataDto
    {
        public int Total { get; set; }
        public SeasonTableDto SeasonTable { get; set; }
    }

    public class SeasonApiResponseDto
    {
        public MRSeasonDataDto MRData { get; set; }
    }

    public async Task<List<SeasonApiDto>> GetSeasonsFromApiAsync()
    {
        int limit = _workerConfig.SyncRequestLimit;
        int offset = 0;
        string queryParams;
        string apiUrl;
        
        List<SeasonApiDto> tempSeasons = new List<SeasonApiDto>();
        
        bool condition = true;
        while (condition)
        {
            await Task.Delay(_workerConfig.DelayBetweenRequests);

            queryParams = $"limit={limit}&offset={offset}";
            apiUrl = $"https://api.jolpi.ca/ergast/f1/seasons/?{queryParams}";

            var response = await _httpClient.GetStringAsync(apiUrl);
            if (string.IsNullOrEmpty(response))
            {
                break;
            }

            SeasonApiResponseDto apiResponse = JsonConvert.DeserializeObject<SeasonApiResponseDto>(response);
            if (apiResponse == null || apiResponse.MRData.SeasonTable.Seasons.Count < 100)
            {
                condition = false;
            }

            if (offset == 0 && apiResponse != null)
            {
                using var scope = _scopeFactory.CreateScope();
                var seasonService = scope.ServiceProvider.GetRequiredService<ISeasonService>();
                int currentSeasonCount = await seasonService.GetSeasonsCountAsync();
                if (apiResponse.MRData.Total == currentSeasonCount)
                {
                    return [];
                }
                offset = currentSeasonCount;
            }
         
            
            Console.WriteLine($"offset: {offset} - Seasons count: {apiResponse.MRData.SeasonTable.Seasons.Count}");
            tempSeasons.AddRange(apiResponse.MRData.SeasonTable.Seasons);

            offset += limit;
        }

        return tempSeasons;
    }
}