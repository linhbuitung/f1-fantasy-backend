using System.Text.Json.Serialization;
using F1Fantasy.Modules.AskAiModule.Dtos.Api;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Create;

public class MainRacePredictionCreateDto
{
    [JsonPropertyName("laps")]
    public int Laps { get; set; }

    [JsonPropertyName("circuit")]
    public string CircuitCode { get; set; }

    [JsonPropertyName("race_date")]
    public DateTime RaceDate { get; set; }

    [JsonPropertyName("rain")]
    public bool Rain { get; set; }
    
    [JsonPropertyName("entries")]
    public List<DriverPredictionInputCreateDto> Entries { get; set; }
}