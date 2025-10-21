using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class PickableDriverApiDto
{
    [JsonPropertyName ("driverRef")]
    public required string DriverRef { get; set; }
    [JsonPropertyName ("driver_nationality")]
    public required string DriverNationality { get; set; }
    [JsonPropertyName ("driver_date_of_birth")]
    public required DateOnly DriverDateOfBirth { get; set; }
    [JsonPropertyName ("first_race_date")]
    public required DateOnly FirstRaceDate { get; set; }
    
}
