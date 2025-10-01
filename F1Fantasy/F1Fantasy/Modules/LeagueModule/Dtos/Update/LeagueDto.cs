using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.LeagueModule.Dtos.Update;

public class LeagueDto
{
    [Required]
    public int Id { get; set; }

    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public int OwnerId { get; set; }
}