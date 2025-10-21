using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Create;

public class DriverPredictionInputCreateDto
{
    public int? QualificationPosition { get; set; }
    
    [Required]
    public int ConstructorId { get; set; }

    [Required]
    public int DriverId { get; set; }
}