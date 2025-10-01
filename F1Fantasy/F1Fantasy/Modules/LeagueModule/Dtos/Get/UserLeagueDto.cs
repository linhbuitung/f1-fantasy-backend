using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.LeagueModule.Dtos.Get;

public class UserLeagueDto
{    
    [Required]
    public int LeagueId { get; set; }
    [Required]
    public int UserId { get; set; }
    
    public string? UserDisplayName { get; set; }
    [Required]
    public string UserEmail { get; set; }

    [Required]
    public bool IsAccepted { get; set; }
}