using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;
using Newtonsoft.Json;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;

public class RaceEntrySyncService : IRaceEntrySyncService
{
    private readonly WorkerConfigurationService _workerConfig;
    private readonly HttpClient _httpClient;
    private readonly IServiceScopeFactory _scopeFactory;

    public RaceEntrySyncService(HttpClient httpClient, IServiceScopeFactory scopeFactory, WorkerConfigurationService workerConfig)
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

    public async Task<List<RaceEntryApiDto>> GetRaceEntriesForCurrentYearFromApiAsync()
    {
        int limit = _workerConfig.SyncRequestLimit;
        int offset = 0;
        string queryParams;
        string apiUrl;
        int year = DateTime.UtcNow.Year;
        
        List<RaceEntryApiDto> tempRaceEntries = new List<RaceEntryApiDto>();

        bool condition = true;
        while (condition)
        {
            await Task.Delay(_workerConfig.DelayBetweenRequests);

            queryParams = $"limit={limit}&offset={offset}";
            apiUrl = $"https://api.jolpi.ca/ergast/f1/{year}/results/?{queryParams}";

            var response = await _httpClient.GetStringAsync(apiUrl);
            if (string.IsNullOrEmpty(response))
            {
                break;
            }

            RaceApiResponseDto apiResponse = JsonConvert.DeserializeObject<RaceApiResponseDto>(response);
            List<RaceEntryApiDto> raceEntriesFound =
                apiResponse.MRData.RaceTable.Races
                    .SelectMany(race => race.Results.Select(entry => {
                        entry.RaceDate = race.Date;
                        return entry;
                    }))
                    .ToList();
            if (apiResponse == null || raceEntriesFound.Count < 100)
            {
                condition = false;
            }

            // This will be commented until number of race entries is stable
            /*if (offset == 0 && apiResponse != null)
            {
                using var scope = _scopeFactory.CreateScope();
                var raceEntryService = scope.ServiceProvider.GetRequiredService<IRaceEntryService>();
                int currentRaceEntriesCount = await raceEntryService.GetRaceEntriesCountAsync();
                if (apiResponse.MRData.Total == currentRaceEntriesCount)
                {
                    return [];
                }
                offset = currentRaceEntriesCount;
            }*/

            Console.WriteLine($"offset: {offset} - Race entries count: {raceEntriesFound.Count}");
            tempRaceEntries.AddRange(raceEntriesFound);

            offset += limit;
        }

        return tempRaceEntries;
    }
    
    public async Task<List<RaceEntryDto>> GetStaticRaceEntriesAsync()
    {
        var result = new List<RaceEntryDto>();
        var filePath = Path.Combine(AppContext.BaseDirectory, "Static", "initial.csv");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"initial.csv not found at {filePath}");

        using var reader = new StreamReader(filePath);
        string? line;
        bool isFirstLine = true;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (isFirstLine)
            {
                isFirstLine = false; // skip header
                continue;
            }

            // Use a simple CSV parser to handle quoted fields with commas
            var fields = ParseCsvLine(line);
            if (fields.Count < 5)
                continue;

            //date,grid,position,rank,driverRef,constructorRef,circuitRef,finished

            // parse to date
            var date = fields[0] == @"\N" ? default : DateOnly.Parse(fields[0]);
            int? grid = fields[1] == @"\N" ? null : Int32.Parse(fields[1]);
            int? position = fields[2] == @"\N" ? null : Int32.Parse(fields[2]);
            int? rank = fields[3] == @"\N" ? null : Int32.Parse(fields[3]);
            var driverRef = fields[4] == @"\N" ? null : fields[4].Trim();
            var constructorRef = fields[5] == @"\N" ? null : fields[5].Trim();
            var circuitRef = fields[6] == @"\N" ? null : fields[6].Trim();
            var finished = fields[7].Trim().ToLower() == "true";
            
            result.Add(new RaceEntryDto(id: null, 
                position, 
                grid,
                rank,
                0,
                null,
                driverRef,
                null,
                date,
                null,
                constructorRef,
                circuitRef,
                finished
            ));
        }

        return result;
    }
    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        var field = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }
        fields.Add(field.ToString());
        return fields;
    }
}