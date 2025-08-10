using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Linq;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services
{
    public class F1DataSyncService : IF1DataSyncService
    {
        private readonly HttpClient _httpClient;

        public F1DataSyncService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region driver

        private class DriverTableDto
        {
            public List<DriverApiDto> Drivers { get; set; }
        }

        private class MRDriverDataDto
        {
            public DriverTableDto DriverTable { get; set; }
        }

        private class DriverApiResponseDto
        {
            public MRDriverDataDto MRData { get; set; }
        }

        public async Task<List<DriverApiDto>> GetDriversAsync()
        {
            int limit = 100;
            int offset = 0;
            string queryParams;
            string apiUrl;

            List<DriverApiDto> tempDrivers = new List<DriverApiDto>();

            bool condition = true;
            while (condition)
            {
                queryParams = $"limit={limit}&offset={offset}";
                apiUrl = $"https://api.jolpi.ca/ergast/f1/drivers/?{queryParams}";

                var response = await _httpClient.GetStringAsync(apiUrl);
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

                offset += limit;
                await Task.Delay(500);
            }

            return tempDrivers;
        }

        #endregion driver

        #region constructor

        private class ConstructorTableDto
        {
            public List<ConstructorApiDto> Constructors { get; set; }
        }

        private class MRConstructorDataDto
        {
            public ConstructorTableDto ConstructorTable { get; set; }
        }

        private class ConstructorApiResponseDto
        {
            public MRConstructorDataDto MRData { get; set; }
        }

        public async Task<List<ConstructorApiDto>> GetConstructorsAsync()
        {
            int limit = 100;
            int offset = 0;
            string queryParams;
            string apiUrl;

            List<ConstructorApiDto> tempConstructors = new List<ConstructorApiDto>();

            bool condition = true;
            while (condition)
            {
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

                offset += limit;
                await Task.Delay(500);
            }

            return tempConstructors;
        }

        #endregion constructor

        #region circuit

        private class CircuitTableDto
        {
            public List<CircuitApiDto> Circuits { get; set; }
        }

        private class MRCircuitDataDto
        {
            public CircuitTableDto CircuitTable { get; set; }
        }

        private class CircuitApiResponseDto
        {
            public MRCircuitDataDto MRData { get; set; }
        }

        public async Task<List<CircuitApiDto>> GetCircuitsAsync()
        {
            int limit = 100;
            int offset = 0;
            string queryParams;
            string apiUrl;

            List<CircuitApiDto> tempCircuits = new List<CircuitApiDto>();

            bool condition = true;
            while (condition)
            {
                queryParams = $"limit={limit}&offset={offset}";
                apiUrl = $"https://api.jolpi.ca/ergast/f1/circuits/?{queryParams}";

                var response = await _httpClient.GetStringAsync(apiUrl);
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

                offset += limit;
                await Task.Delay(500);
            }

            return tempCircuits;
        }

        #endregion circuit

        #region nationality

        public async Task<List<CountryDto>> GetNationalitiesAsync()
        {
            var result = new List<CountryDto>();
            var filePath = Path.Combine(AppContext.BaseDirectory, "Static", "countries.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"countries.csv not found at {filePath}");

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

                var alpha3 = fields[2].Trim();
                var shortName = fields[3].Trim();
                var nationalityField = fields[4].Trim();

                // Remove surrounding quotes if present
                if (nationalityField.StartsWith("\"") && nationalityField.EndsWith("\""))
                    nationalityField = nationalityField[1..^1];

                // Split by comma, then by "or", and trim each name
                var nationalities = nationalityField
                    .Split(',')
                    .SelectMany(n => n.Split(new[] { " or " }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(n => n.Trim())
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList();

                if (!string.IsNullOrEmpty(alpha3) && nationalities.Count > 0)
                    result.Add(new CountryDto(alpha3, nationalities, shortName));
            }

            return result;
        }

        // Simple CSV parser for quoted fields
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

        #endregion nationality
    }
}