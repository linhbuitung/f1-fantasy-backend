using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class StatusPredictInputDto
{
    [JsonPropertyName("qualification_position")]
    public int QualificationPosition { get; set; }

    [JsonPropertyName("constructor")]
    public string ConstructorCode { get; set; }

    [JsonPropertyName("circuit")]
    public string CircuitCode { get; set; }

    [JsonPropertyName("driver")]
    public string DriverCode { get; set; }

    [JsonPropertyName("race_date")]
    public DateOnly RaceDate { get; set; }

    [JsonPropertyName("rain")]
    public int? Rain { get; set; }
}

public class StatusPredictionItemDto
{
    [JsonPropertyName("input")]
    public StatusPredictInputDto Input { get; set; }

    // Features can contain mixed types; use JsonElement or object.
    [JsonPropertyName("features")]
    public Dictionary<string, JsonElement> Features { get; set; }

    [JsonPropertyName("dnf_percentage")]
    public double PredictedDnfPercentage { get; set; }
    
}

public class StatusPredictResponseDto
{
    [JsonPropertyName("percentages")]
    public List<StatusPredictionItemDto> Percentages { get; set; }

    [JsonPropertyName("model_meta")]
    public Dictionary<string, JsonElement> ModelMeta { get; set; }
}