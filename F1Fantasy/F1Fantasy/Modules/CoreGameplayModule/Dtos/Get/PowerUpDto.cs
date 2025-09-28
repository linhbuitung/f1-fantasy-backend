
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

public class PowerupDto
{
    public int? Id { get; set; } 

    public string Type { get; set; }  = null!;

    public string Description { get; set; } = null!;
    [JsonConverter(typeof(StringEnumConverter))]
    public Status Status { get; set; }


    public string ImgUrl { get; set; } = null!;
}

public enum Status
{
    Used,
    Using,
    Available
}