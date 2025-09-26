using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

public class DriverRaceResultDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string GivenName { get; set; }

    [Required, MaxLength(300)]
    public string FamilyName { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; }
    
    [Required]
    public int PointGained { get; set; }

    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}