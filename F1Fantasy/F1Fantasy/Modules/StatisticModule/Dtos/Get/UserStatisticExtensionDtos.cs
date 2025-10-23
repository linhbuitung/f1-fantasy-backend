namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class UserWithAveragePointScoredGetDto : UserStatisticBaseGetDto
{
    public double AverageFantasyPointScored { get; set; }

}
public class UserWithTotalFantasyPointScoredGetDto : UserStatisticBaseGetDto
{
    public int TotalFantasyPointScored { get; set; }
}