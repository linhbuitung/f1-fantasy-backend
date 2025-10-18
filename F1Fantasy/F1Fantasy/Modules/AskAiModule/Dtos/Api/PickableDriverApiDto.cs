using Newtonsoft.Json;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class PickableDriverApiDto
{
    [JsonProperty ("driverRef")]
    public required string DriverRef { get; set; }
    [JsonProperty ("driver_nationality")]
    public required string DriverNationality { get; set; }
    [JsonProperty ("driver_date_of_birth")]
    public required DateOnly DriverDateOfBirth { get; set; }
    [JsonProperty ("first_race_date")]
    public required DateOnly FirstRaceDate { get; set; }
    
}
