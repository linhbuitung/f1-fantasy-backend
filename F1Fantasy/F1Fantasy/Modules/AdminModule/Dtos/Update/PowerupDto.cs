using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Update;

public class PowerupDto
{
    [Required]
    public int Id { get; set; }
    [MaxLength(300)]
    public string ImgUrl { get; set; }
}