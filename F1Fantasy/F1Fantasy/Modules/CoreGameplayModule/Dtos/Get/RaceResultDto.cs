using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

public class RaceResultDto
{
    [Required]
    public RaceDto Race { get; set; }
    
    public List<DriverRaceResultDto> DriverResults { get; set; }
    
    public List<ConstructorRaceResultDto> ConstructorResults { get; set; }
}