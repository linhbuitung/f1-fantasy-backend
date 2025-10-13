using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class ConstructorInDriverPredictionGetDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Name { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; }
    
    [MaxLength(300)]
    public string? ImgUrl { get; set; }
}