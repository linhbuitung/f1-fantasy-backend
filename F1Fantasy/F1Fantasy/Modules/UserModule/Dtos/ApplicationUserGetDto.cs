using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.UserModule.Dtos;

public class ApplicationUserGetDto
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string? DisplayName { get; set; }
    
    public string Email { get; set; }

    [Column(TypeName = "date")]
    public DateOnly? DateOfBirth { get; set; }

    public bool? AcceptNotification { get; set; }

    public int? LoginStreak { get; set; }

    public DateTime? LastLogin { get; set; }

    public int? ConstructorId { get; set; }

    public string? ConstructorName { get; set; } 
    
    public int? DriverId { get; set; }

    public string? DriverName { get; set; }
    
    public string? CountryId { get; set; }

    public string? CountryName { get; set; }
}