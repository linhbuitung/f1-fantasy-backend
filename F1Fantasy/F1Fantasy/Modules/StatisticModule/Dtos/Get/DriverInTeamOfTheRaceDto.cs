namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class DriverInTeamOfTheRaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int PointGained { get; set; }
    
    public string? ImgUrl { get; set; }
}