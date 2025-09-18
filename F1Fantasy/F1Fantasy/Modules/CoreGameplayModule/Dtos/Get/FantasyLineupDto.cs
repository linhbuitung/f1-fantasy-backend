using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

public class FantasyLineupDto
{    
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int TotalAmount { get; set; }
    
    [Required]
    public int TransfersMade { get; set; }
    
    [Required]
    public int PointsGained { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int RaceId { get; set; }

    public List<Dtos.Get.DriverInFantasyLineupDto> Drivers { get; set; }
    
    public List<Dtos.Get.ConstructorInFantasyLineupDto> Constructors { get; set; }
    
    public List<Dtos.Get.PowerupInFantasyLineupDto> Powerups { get; set; }
}