using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using F1FantasyWorker.Core.Common;

namespace F1FantasyWorker.Modules.StaticDataModule.Dtos;

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