using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Update;

public class FantasyLineupDto
{    
    [Required]
    public int Id { get; set; }

    public List<int> DriverIds { get; set; }
    
    public List<int> ConstructorIds { get; set; }
    
    public List<int> PowerupIds { get; set; }
}