using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;
using Newtonsoft.Json;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;

public class ConstructorSyncService : IConstructorSyncService
{
    private readonly WorkerConfigurationService _workerConfig;
    private readonly HttpClient _httpClient;
    private readonly IServiceScopeFactory _scopeFactory;

    public ConstructorSyncService(HttpClient httpClient, IServiceScopeFactory scopeFactory, WorkerConfigurationService workerConfig)
    {
        _scopeFactory = scopeFactory;
        _httpClient = httpClient;
        _workerConfig = workerConfig;
    }
    
    public class ConstructorTableDto
    {
        public List<ConstructorApiDto> Constructors { get; set; }
    }

    public class MRConstructorDataDto
    {
        public int Total { get; set; }
        public ConstructorTableDto ConstructorTable { get; set; }
    }

    public class ConstructorApiResponseDto
    {
        public MRConstructorDataDto MRData { get; set; }
    }

    public async Task<List<ConstructorApiDto>> GetConstructorsFromApiAsync()
    {
        int limit = _workerConfig.SyncRequestLimit;
        int offset = 0;
        string queryParams;
        string apiUrl;
        
        List<ConstructorApiDto> tempConstructors = new List<ConstructorApiDto>();

        bool condition = true;
        while (condition)
        {
            await Task.Delay(_workerConfig.DelayBetweenRequests);

            queryParams = $"limit={limit}&offset={offset}";
            apiUrl = $"https://api.jolpi.ca/ergast/f1/constructors/?{queryParams}";

            var response = await _httpClient.GetStringAsync(apiUrl);
            if (string.IsNullOrEmpty(response))
            {
                break;
            }

            ConstructorApiResponseDto apiResponse = JsonConvert.DeserializeObject<ConstructorApiResponseDto>(response);
           
            if (apiResponse == null || apiResponse.MRData.ConstructorTable.Constructors.Count < 100)
            {
                condition = false;
            }
            
            Console.WriteLine($"offset: {offset} - Constructors count: {apiResponse.MRData.ConstructorTable.Constructors.Count}");
            tempConstructors.AddRange(apiResponse.MRData.ConstructorTable.Constructors);

            if (offset == 0 && apiResponse != null)
            {
                using var scope = _scopeFactory.CreateScope();
                var constructorService = scope.ServiceProvider.GetRequiredService<IConstructorService>();
                int currentConstructorCount = await constructorService.GetConstructorsCountAsync();
                if (apiResponse.MRData.Total == currentConstructorCount)
                {
                    return [];
                }

                if (currentConstructorCount == 0)
                {
                    offset += limit;
                }
                else
                {
                    offset = currentConstructorCount;

                }
            }
            else
            {
                offset += limit;
            }
        }

        return tempConstructors;
    }
}