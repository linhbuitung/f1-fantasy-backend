using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Get;

public class DriverDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string GivenName { get; set; }

    [Required, MaxLength(300)]
    public string FamilyName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [MaxLength(200)]
    public string? CountryId { get; set; }

    [MaxLength(50)]
    public string Code { get; set; }
    
    [Required, Range(1,100)]
    public int Price { get; set; }

    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}