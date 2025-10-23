namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class DriverWithAveragePointScoredGetDto : DriverStatisticBaseGetDto
{
    public double AverageFantasyPointScored { get; set; }

}

public class DriverWithSelectionPercentageGetDto : DriverStatisticBaseGetDto
{
    public double SelectionPercentage { get; set; } 
}

public class DriverWithTotalFantasyPointScoredGetDto : DriverStatisticBaseGetDto
{
    public int TotalFantasyPointScored { get; set; }

}

public class DriverWithRaceWinsGetDto : DriverStatisticBaseGetDto
{
    public int TotalRacesWin { get; set; }
}

public class DriverWithPodiumsGetDto : DriverStatisticBaseGetDto
{
    public int TotalPodiums { get; set; }
}

public class DriverWithTop10FinishesGetDto : DriverStatisticBaseGetDto
{
    public int TotalTop10Finishes { get; set; }
}

public class DriverWithFastestLapsGetDto : DriverStatisticBaseGetDto
{
    public int TotalFastestLaps { get; set; }
}

public class DriverWithDnfsGetDto : DriverStatisticBaseGetDto
{
    public int TotalDnfs { get; set; }
}