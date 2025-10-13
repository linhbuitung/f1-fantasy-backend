using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using F1Fantasy.Modules.AskAiModule.Dtos.Api;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Create;

public class MainRacePredictionCreateAsNewDto
{
    [Required]
    public int Laps { get; set; }

    [Required]
    public int CircuitId { get; set; }

    [Required]
    public DateTime RaceDate { get; set; }
    
    [Required]
    public DateTime QualifyingDate { get; set; }

    [Required]
    public bool Rain { get; set; }
    
    public List<DriverPredictionInputCreateDto> Entries { get; set; }
}