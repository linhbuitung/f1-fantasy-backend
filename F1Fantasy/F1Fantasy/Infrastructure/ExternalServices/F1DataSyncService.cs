using F1Fantasy.Infrastructure.ExternalServices.TempModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Linq;

namespace F1Fantasy.Infrastructure.ExternalServices
{
    public class F1DataSyncService
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
    }
}