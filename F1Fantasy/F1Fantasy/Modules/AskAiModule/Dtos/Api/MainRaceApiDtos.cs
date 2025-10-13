using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class MainRacePredictInputDto
{
    [JsonPropertyName("qualification_position")]
    public int QualificationPosition { get; set; }

    [JsonPropertyName("laps")]
    public int Laps { get; set; }

    [JsonPropertyName("constructor")]
    public string ConstructorCode { get; set; }

    [JsonPropertyName("circuit")]
    public string CircuitCode { get; set; }

    [JsonPropertyName("driver")]
    public string DriverCode { get; set; }

    [JsonPropertyName("race_date")]
    public DateTime RaceDate { get; set; }

    [JsonPropertyName("rain")]
    [JsonConverter(typeof(BoolToZeroOneConverter<bool>))]

    public bool? Rain { get; set; }
}

public class MainRacePredictionItemDto
{
    [JsonPropertyName("input")]
    public MainRacePredictInputDto Input { get; set; }

    // Features can contain mixed types; use JsonElement or object.
    [JsonPropertyName("features")]
    public Dictionary<string, JsonElement> Features { get; set; }

    [JsonPropertyName("predicted_deviation_from_median")]
    public double PredictedDeviationFromMedian { get; set; }

    [JsonPropertyName("predicted_final_position")]
    public int PredictedFinalPosition { get; set; }
}

public class MainRacePredictResponseDto
{
    [JsonPropertyName("predictions")]
    public List<MainRacePredictionItemDto> Predictions { get; set; }

    [JsonPropertyName("model_meta")]
    public Dictionary<string, JsonElement> ModelMeta { get; set; }
}