using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.LeagueModule.Dtos.Get;

public class UserInLeagueDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Email { get; set; }

    [MaxLength(100)]
    public string? DisplayName { get; set; }
    
    public string? CountryName { get; set; }
    
    public int TotalPoints { get; set; }
}