using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Update;

public class DriverDto
{
    [Required]
    public int Id { get; set; }
    [Range(1,100)]
    public int? Price { get; set; }

    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}