namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class UserStatisticBaseGetDto
{
    public int Id { get; set; }
    public string? DisplayName { get; set; }
    
    public string Email { get; set; }
    
    public DateOnly JoinDate { get; set; }
}