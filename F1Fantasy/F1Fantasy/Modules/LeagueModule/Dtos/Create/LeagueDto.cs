using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.LeagueModule.Dtos.Create;

public class LeagueDto
{
    [Required]
    public int MaxPlayersNum { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    public int OwnerId { get; set; }
}