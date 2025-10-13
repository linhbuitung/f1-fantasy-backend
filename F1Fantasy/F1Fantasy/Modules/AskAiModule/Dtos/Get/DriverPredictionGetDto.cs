using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class DriverPredictionGetDto
{
    public int Id { get; set; }

    public int? GridPosition { get; set; }

    public int? FinalPosition { get; set; }

    [Required]
    public bool Crashed { get; set; }

    // Foreign keys and navigation properties
    [Required]
    public int PredictionId { get; set; }

    public required DriverInDriverPreditionGetDto Driver { get; set; }
    
    public required ConstructorInDriverPredictionGetDto Constructor { get; set; }
}