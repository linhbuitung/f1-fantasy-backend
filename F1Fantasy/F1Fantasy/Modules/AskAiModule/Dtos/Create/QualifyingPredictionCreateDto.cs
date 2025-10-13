using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using F1Fantasy.Modules.AskAiModule.Dtos.Api;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Create;

public class QualifyingPredictionCreateDto
{
    [Required]
    public int CircuitId { get; set; }
    
    [Required]
    public DateTime QualifyingDate { get; set; }
    
    [Required]
    public List<DriverPredictionInputCreateDto> Entries { get; set; }
}