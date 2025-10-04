using System.ComponentModel.DataAnnotations;
using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.LeagueModule.Dtos.Get;

public class LeagueDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int MaxPlayersNum { get; set; }

    [Required]
    public LeagueType Type { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }
    
    public int TotalPlayersNum { get; set; }

    [Required]
    public UserInLeagueDto? Owner { get; set; }
    
    public List<UserInLeagueDto>? Users { get; set; }
}