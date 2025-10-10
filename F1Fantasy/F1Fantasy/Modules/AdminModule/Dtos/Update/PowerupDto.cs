using System.ComponentModel.DataAnnotations;
using F1Fantasy.Modules.AdminModule.Dtos.Validations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Update;

public class PowerupDto
{
    [Required]
    public int Id { get; set; }
    [MaxFileSize(5 * (1024 * 1024))] // 5 MB
    [PermittedExtensions([".jpg", ".jpeg", ".png"])]
    public required IFormFile Img{ get; set; }
}