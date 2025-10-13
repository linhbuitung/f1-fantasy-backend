using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class DriverInDriverPreditionGetDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string GivenName { get; set; }

    [Required, MaxLength(300)]
    public string FamilyName { get; set; }
    
    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; }

    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}