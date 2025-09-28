namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class BestRaceWeekOfAnUserDto
{

    public int FantasyLineupId { get; set; }

    public string RaceName { get; set; } = null!;

    public int PointsGained { get; set; }

    public DateOnly RaceDate { get; set; }

}