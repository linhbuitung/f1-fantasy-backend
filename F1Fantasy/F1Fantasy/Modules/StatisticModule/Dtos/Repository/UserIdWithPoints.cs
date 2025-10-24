namespace F1Fantasy.Modules.StatisticModule.Dtos.Repository;

// User for repository methods
public class UserIdWithPoints
{
    public int UserId { get; set; }
    public int? TotalPoints { get; set; }
    public double? AveragePoints { get; set; }
}