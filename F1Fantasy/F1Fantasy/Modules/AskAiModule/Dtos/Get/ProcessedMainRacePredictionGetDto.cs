namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class ProcessedMainRacePredictionGetDto
{
    public int GridPosition { get; set; }

    public int FinalPosition { get; set; }

    public int ConstructorId { get; set; }
    
    public int DriverId { get; set; }
}