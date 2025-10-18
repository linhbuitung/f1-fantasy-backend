using Newtonsoft.Json;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Api;

public class PickableConstructorApiDto
{
    [JsonProperty ("constructorRef")]
    public required string ConstructorRef { get; set; }
    [JsonProperty ("constructor_nationality")]
    public required string ConstructorNationality { get; set; }

}