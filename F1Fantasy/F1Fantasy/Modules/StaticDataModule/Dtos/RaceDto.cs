using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StaticDataModule.Dtos;

public class RaceDto
{
  
    public int? Id { get; set; }
    
    [Required]
    public required string RaceName { get; set; }

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
}