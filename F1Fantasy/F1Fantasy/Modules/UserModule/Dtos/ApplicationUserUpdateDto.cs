using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.UserModule.Dtos;

public class ApplicationUserUpdateDto
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string? DisplayName { get; set; }

    [Column(TypeName = "date")]
    public DateOnly? DateOfBirth { get; set; }

    public bool AcceptNotification { get; set; }
    
    public int? ConstructorId { get; set; }
    

    public int? DriverId { get; set; }
    
    public string? CountryId { get; set; }
}