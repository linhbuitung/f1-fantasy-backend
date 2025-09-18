using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Get;

public class PickableItemDto
{
    public List<DriverInPickableItemDto> Drivers { get; set; }
    public List<ConstructorInPickableItemDto> Constructors { get; set; }
}

public class DriverInPickableItemDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string GivenName { get; set; }

    [Required, MaxLength(300)]
    public string FamilyName { get; set; }

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required, MaxLength(200)]
    public string CountryId { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; }
    
    [Required, Range(1,100)]
    public int Price { get; set; }

    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}

public class ConstructorInPickableItemDto
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