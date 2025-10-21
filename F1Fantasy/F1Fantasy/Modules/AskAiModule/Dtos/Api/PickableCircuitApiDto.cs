using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class PickableCircuitApiDto
{
    [JsonPropertyName ("circuitRef")]
    public required string CircuitRef { get; set; }
    [JsonPropertyName ("circuit_nationality")]
    public required string CircuitNationality { get; set; }
}