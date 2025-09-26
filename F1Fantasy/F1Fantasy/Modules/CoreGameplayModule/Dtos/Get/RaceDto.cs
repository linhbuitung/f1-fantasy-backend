using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

public class RaceDto
{    
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string RaceName { get; set; }

    [Required]
    public int Round { get; set; }

    [Required]
    public DateOnly RaceDate { get; set; }

    [Required]
    public DateOnly DeadlineDate { get; set; }

    [Required]
    public bool Calculated { get; set; }


    public int SeasonYear { get; set; }

    public int CircuitId { get; set; }
    
    public string CircuitName { get; set; }

    public string CircuitCode { get; set; }
}