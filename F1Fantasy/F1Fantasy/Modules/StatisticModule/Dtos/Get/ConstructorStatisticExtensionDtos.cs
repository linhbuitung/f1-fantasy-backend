namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class ConstructorWithAveragePointScoredGetDto : ConstructorStatisticBaseGetDto
{
    public double AverageFantasyPointScored { get; set; }

}

public class ConstructorWithSelectionPercentageGetDto : ConstructorStatisticBaseGetDto
{
    public double SelectionPercentage { get; set; } 
}

public class ConstructorWithTotalFantasyPointScoredGetDto : ConstructorStatisticBaseGetDto
{
    public int TotalFantasyPointScored { get; set; }

}

public class ConstructorWithPodiumsGetDto : ConstructorStatisticBaseGetDto
{
    public int TotalPodiums { get; set; }
}

public class ConstructorWithTop10FinishesGetDto : ConstructorStatisticBaseGetDto
{
    public int TotalTop10Finishes { get; set; }
}