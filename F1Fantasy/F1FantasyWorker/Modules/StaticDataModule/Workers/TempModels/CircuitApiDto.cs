using F1FantasyWorker.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels
{
    public class CircuitApiDto(
        string circuitId,
        string circuitName,
        string url,
        decimal lat,
        decimal lng,
        string locality,
        string country)
    {
        public string CircuitId { get; set; } = circuitId;
        public string CircuitName { get; set; } = circuitName;
        public string Url { get; set; } = url;

        [JsonProperty("Location")]
        public LocationApiDto Location { get; set; } = new(lat, lng, locality, country);

        public override string ToString()
        {
            string result = CircuitId + "," + CircuitName + "," + Url + "," + Location.ToString();
            return result;
        }

        public class LocationApiDto(decimal lat, decimal lng, string locality, string country)
        {
            [JsonProperty("lat")]
            public decimal Lat { get; set; } = lat;

            [JsonProperty("long")]
            public decimal Long { get; set; } = lng;

            [JsonProperty("locality")]
            public string Locality { get; set; } = locality;

            [JsonProperty("country")]
            public string Country { get; set; } = country;

            public override string ToString()
            {
                string result = Lat + "," + Long + "," + Locality + "," + Country;
                return result;
            }
        }
    }
}