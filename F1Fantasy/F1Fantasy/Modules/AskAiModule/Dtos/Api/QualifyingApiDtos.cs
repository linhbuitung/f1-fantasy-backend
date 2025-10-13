using System.Text.Json;
using System.Text.Json.Serialization;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class QualifyingPredictInputDto
{
    [JsonPropertyName("constructor")]
    public string ConstructorCode { get; set; }

    [JsonPropertyName("circuit")]
    public string CircuitCode { get; set; }

    [JsonPropertyName("driver")]
    public string DriverCode { get; set; }

    [JsonPropertyName("race_date")]
    public DateOnly RaceDate { get; set; }
}

public class QualifyingPredictionItemDto
{
    [JsonPropertyName("input")]
    public QualifyingPredictInputDto Input { get; set; }

    // Features can contain mixed types; use JsonElement or object.
    [JsonPropertyName("features")]
    public Dictionary<string, JsonElement> Features { get; set; }

    [JsonPropertyName("predicted_deviation_from_median")]
    public double PredictedDeviationFromMedian { get; set; }

    [JsonPropertyName("predicted_final_position")]
    public int PredictedFinalPosition { get; set; }
}

public class QualifyingPredictResponseDto
{
    [JsonPropertyName("predictions")]
    public List<QualifyingPredictionItemDto> Predictions { get; set; }

    [JsonPropertyName("model_meta")]
    public Dictionary<string, JsonElement> ModelMeta { get; set; }
}