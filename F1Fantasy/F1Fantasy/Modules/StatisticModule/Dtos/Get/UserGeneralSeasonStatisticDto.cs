namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class UserGeneralSeasonStatisticDto
{
    public int TotalPointsGained { get; set; }
    
    public int TotalTransfersMade { get; set; }
    
    public int OverallRank { get; set; }
    
    public BestRaceWeekOfAnUserDto BestRaceWeek { get; set; }
}

