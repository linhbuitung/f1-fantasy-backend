using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class PickableConstructorApiDto
{
    [JsonPropertyName ("constructorRef")]
    public required string ConstructorRef { get; set; }
    [JsonPropertyName ("constructor_nationality")]
    public required string ConstructorNationality { get; set; }

}