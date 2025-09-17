using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StaticDataModule.Dtos;

public class RaceDto(
    int? id,
    DateOnly raceDate,
    int round,
    DateOnly deadlineDate,
    bool calculated,
    int? seasonId,
    int? circuitId,
    string? circuitCode)
{
  
    public int? Id { get; set; } = id ?? null;

    [Required]
    public int Round { get; set; } = round;

    [Required]
    public DateOnly RaceDate { get; set; } = raceDate;

    [Required]
    public DateOnly DeadlineDate { get; set; } = deadlineDate;

    [Required]
    public bool Calculated { get; set; } = calculated;


    public int? SeasonId { get; set; } = seasonId;

    public int? CircuitId { get; set; } = circuitId;

    public string? CircuitCode { get; set; } = circuitCode;
}