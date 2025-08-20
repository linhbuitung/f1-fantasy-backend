using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos;

public class ApplicationUserForAdminGetDto
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string? DisplayName { get; set; }
    
    public string Email { get; set; }
    
    public List<string> Roles { get; set; } 
}