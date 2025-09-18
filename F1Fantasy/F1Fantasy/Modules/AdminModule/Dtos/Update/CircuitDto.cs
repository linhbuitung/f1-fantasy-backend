using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Update;

public class CircuitDto
{
    [Required]
    public int Id { get; set; }
    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}