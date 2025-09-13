using System.ComponentModel.DataAnnotations;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Dtos;

public class RaceDto
{
      
    public int? Id { get; set; }

    [Required]
    public int Round { get; set; }
    
    [Required]
    public DateOnly RaceDate { get; set; }

    [Required]
    public DateOnly DeadlineDate { get; set; }

    [Required]
    public bool Calculated { get; set; }

    public int? SeasonId { get; set; }

    public int? CircuitId { get; set; }
    
    public string? CircuitCode { get; set; }
    
    List<RaceEntryDto> RaceEntries { get; set; } = new List<RaceEntryDto>();

    public RaceDto(int? id, DateOnly raceDate, int round,  DateOnly deadlineDate, bool calculated, int? seasonId, int? circuitId, string? circuitCode, List<RaceEntryDto> raceEntries)
    {
        Id = id ?? null;
        RaceDate = raceDate;
        Round = round;
        DeadlineDate = deadlineDate;
        Calculated = calculated;
        SeasonId = seasonId;
        CircuitId = circuitId;
        CircuitCode = circuitCode;
        RaceEntries = raceEntries;
    }
}