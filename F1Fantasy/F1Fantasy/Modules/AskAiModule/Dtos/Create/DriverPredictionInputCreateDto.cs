using System.Text.Json.Serialization;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Create;

public class DriverPredictionInputCreateDto
{
    [JsonPropertyName("qualification_position")]
    public int? QualificationPosition { get; set; }
    
    [JsonPropertyName("constructor")]
    public string ConstructorCode { get; set; }

    [JsonPropertyName("driver")]
    public string DriverCode { get; set; }
}