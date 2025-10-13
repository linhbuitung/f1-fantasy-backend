namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class ProcessedStatusGetDto
{
    public int DriverId { get; set; }
    
    public int ConstructorId { get; set; }
    
    public bool Crashed { get; set; }
}