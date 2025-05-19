using F1Fantasy.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;

namespace F1Fantasy.Infrastructure.ExternalServices.TempModels
{
    public class CircuitApiDto
    {
        public string CircuitId { get; set; }
        public string CircuitName { get; set; }
        public string Url { get; set; }

        [JsonProperty("Location")]
        public LocationApiDto Location { get; set; }

        public CircuitApiDto(string circuitId, string circuitName, string url, decimal lat, decimal lng, string locality, string country)
        {
            CircuitId = circuitId;
            CircuitName = circuitName;
            Url = url;
            Location = new LocationApiDto(lat, lng, locality, country);
        }

        public override string ToString()
        {
            string result = CircuitId + "," + CircuitName + "," + Url + "," + Location.ToString();
            return result;
        }

        public class LocationApiDto
        {
            [JsonProperty("lat")]
            public decimal Lat { get; set; }

            [JsonProperty("long")]
            public decimal Long { get; set; }

            [JsonProperty("locality")]
            public string Locality { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            public LocationApiDto(decimal lat, decimal lng, string locality, string country)
            {
                Lat = lat;
                Long = lng;
                Locality = locality;
                Country = country;
            }

            public override string ToString()
            {
                string result = Lat + "," + Long + "," + Locality + "," + Country;
                return result;
            }
        }
    }
}