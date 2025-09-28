namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class GeneralSeasonStatisticDto
{
    public int HighestPoint { get; set; }
    
    public int TotalTransferMade { get; set; }
    
    public string MostPickedDriver { get; set; } = null!;
    
    public double AveragePoints { get; set; }
}