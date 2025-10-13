using System.Text.Json.Serialization;
using F1Fantasy.Modules.AskAiModule.Dtos.Api;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Create;

public class QualifyingPredictionCreateDto
{
    [JsonPropertyName("circuit")]
    public string CircuitCode { get; set; }

    [JsonPropertyName("race_date")]
    public DateTime RaceDate { get; set; }
    
    [JsonPropertyName("entries")]
    public List<DriverPredictionInputCreateDto> Entries { get; set; }
}