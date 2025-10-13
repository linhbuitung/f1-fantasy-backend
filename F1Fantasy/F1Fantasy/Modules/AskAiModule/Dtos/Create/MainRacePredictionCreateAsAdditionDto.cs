using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Create;

public class MainRacePredictionCreateAsAdditionDto
{
    [Required]
    public int Laps { get; set; }
    
    [Required]
    public DateTime RaceDate { get; set; }

    [Required]
    public bool Rain { get; set; }
}