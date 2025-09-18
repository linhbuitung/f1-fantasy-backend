using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Get;

public class ConstructorDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Name { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; }
    
    [Required, Range(1,100)]
    public int Price { get; set; }
    
    [MaxLength(300)]
    public string? ImgUrl { get; set; }
    
    [Required]
    public string CountryId { get; set; }
}