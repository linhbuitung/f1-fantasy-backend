using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Get;

public class PowerupDto
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