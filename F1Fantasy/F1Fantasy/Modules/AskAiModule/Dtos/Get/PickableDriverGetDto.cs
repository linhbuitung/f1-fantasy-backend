namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class PickableDriverGetDto
{
    public int Id { get; set; }
    
    public required string GivenName { get; set; }
    
    public required string FamilyName { get; set; }
    
    public DateOnly DateOfBirth { get; set; }
    
    public required string Code { get; set; }
}