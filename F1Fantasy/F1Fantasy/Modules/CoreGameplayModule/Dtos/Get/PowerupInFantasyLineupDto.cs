using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

public class PowerupInFantasyLineupDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Type { get; set; }

    [Required, MaxLength(500)]
    public string Description { get; set; }

    [Required, MaxLength(300)]
    public string ImgUrl { get; set; }
}