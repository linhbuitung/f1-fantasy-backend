using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StaticDataModule.Dtos;

public class RaceDto
{
  
    public int? Id { get; set; }

    [Required]
    public DateOnly RaceDate { get; set; }

    [Required]
    public DateOnly DeadlineDate { get; set; }

    [Required]
    public bool Calculated { get; set; }


    public int? CircuitId { get; set; }
    
    public string? CircuitCode { get; set; }

    public RaceDto(int? id, DateOnly raceDate, DateOnly deadlineDate, bool calculated, int? circuitId, string? circuitCode)
    {
        Id = id ?? null;
        RaceDate = raceDate;
        DeadlineDate = deadlineDate;
        Calculated = calculated;
        CircuitId = circuitId;
        CircuitCode = circuitCode;
    }
}