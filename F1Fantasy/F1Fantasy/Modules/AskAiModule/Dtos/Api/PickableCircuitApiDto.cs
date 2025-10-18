using Newtonsoft.Json;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class PickableCircuitApiDto
{
    [JsonProperty ("circuitRef")]
    public required string CircuitRef { get; set; }
    [JsonProperty ("circuit_nationality")]
    public required string CircuitNationality { get; set; }
}