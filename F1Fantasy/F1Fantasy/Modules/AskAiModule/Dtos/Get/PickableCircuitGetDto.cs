namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class PickableCircuitGetDto
{
    public int Id { get; set; }

    public required string CircuitName { get; set; }

    public required string Code { get; set; }
}