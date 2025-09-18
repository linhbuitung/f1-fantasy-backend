using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Get;

public class CircuitDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string CircuitName { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; }

    [Required]
    public decimal Latitude { get; set; }

    [Required]
    public decimal Longitude { get; set; }

    [Required, MaxLength(200)]
    public string Locality { get; set; }

    [Required]
    public string CountryId { get; set; }
    
    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}