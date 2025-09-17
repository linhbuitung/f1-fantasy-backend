using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;
using Newtonsoft.Json;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;
 
public class CircuitSyncService(
    HttpClient httpClient,
    IServiceScopeFactory scopeFactory,
    WorkerConfigurationService workerConfig)
    : ICircuitSyncService
{
    public class CircuitTableDto
    {
        public List<CircuitApiDto> Circuits { get; set; }
    }

    public class MRCircuitDataDto
    {
        public int Total { get; set; }
        public CircuitTableDto CircuitTable { get; set; }
    }

    public class CircuitApiResponseDto
    {
        public MRCircuitDataDto MRData { get; set; }
    }

    public async Task<List<CircuitApiDto>> GetCircuitsFromApiAsync()
    {
        int limit = workerConfig.SyncRequestLimit;
        int offset = 0;
        string queryParams;
        string apiUrl;
        
        List<CircuitApiDto> tempCircuits = new List<CircuitApiDto>();

        bool condition = true;
        while (condition)
        {
            await Task.Delay(workerConfig.DelayBetweenRequests);

            queryParams = $"limit={limit}&offset={offset}";
            apiUrl = $"https://api.jolpi.ca/ergast/f1/circuits/?{queryParams}";

            var response = await httpClient.GetStringAsync(apiUrl);
            if (string.IsNullOrEmpty(response))
            {
                break;
            }
            CircuitApiResponseDto apiResponse = JsonConvert.DeserializeObject<CircuitApiResponseDto>(response);

            if (apiResponse == null || apiResponse.MRData.CircuitTable.Circuits.Count < 100)
            {
                condition = false;
            }
            
            tempCircuits.AddRange(apiResponse.MRData.CircuitTable.Circuits);
            
            if (offset == 0 && apiResponse != null)
            {
                using var scope = scopeFactory.CreateScope();
                var circuitService = scope.ServiceProvider.GetRequiredService<ICircuitService>();
                int currentCircuitsCount = await circuitService.GetCircuitsCountAsync();
                if (apiResponse.MRData.Total == currentCircuitsCount)
                {
                    return [];
                }

                offset = currentCircuitsCount;
            }
            else
            {
                offset += limit;
            }

        }

        return tempCircuits;
    }
}