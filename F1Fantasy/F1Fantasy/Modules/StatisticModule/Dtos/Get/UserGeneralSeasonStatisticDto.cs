namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class UserGeneralSeasonStatisticDto
{
    public int TotalPointsGained { get; set; }
}

public class BestRaceWeekDto
{
    public int Id { get; set; }
    
    public int Round { get; set; }
    
    public DateOnly RaceDate { get; set; }
    
    public string CircuitName { get; set; } = null!;
    
    public int PointsGained { get; set; }
}