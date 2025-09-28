using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

public class ConstructorRaceResultDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Name { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; }
    
    [Required]
    public int PointGained { get; set; }
    
    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}