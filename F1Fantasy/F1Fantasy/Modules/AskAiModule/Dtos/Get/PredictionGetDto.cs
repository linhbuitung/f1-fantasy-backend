using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class PredictionGetDto
{
    public int Id { get; set; }

    [Required]

    public DateOnly DatePredicted { get; set; }

    public DateTime? RaceDate { get; set; }

    [Required]
    public DateTime QualifyingDate { get; set; }

    [Required]
    public bool Rain { get; set; }

    public int? Laps { get; set; }

    [Required]
    public int UserId { get; set; }
    
    public bool IsQualifyingCalculated { get; set; }
    
    public bool IsRaceCalculated { get; set; }

    public required CircuitInPredictionGetDto Circuit { get; set; }

    public List<DriverPredictionGetDto>? DriverPredictions { get; set; }
}