namespace F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

public class RaceApiDto
{
    public int Season { get; set; }
    
    public int Round { get; set; }
    
    public string Url { get; set; }
    
    public string RaceName { get; set; }
    
    public CircuitApiDto Circuit { get; set; }
    
    public DateOnly Date  { get; set; }

    public RaceApiDto(int season, int round, string url, string raceName, CircuitApiDto circuit, DateOnly date)
    {
        Season = season;
        Round = round;
        Url = url;
        RaceName = raceName;
        Circuit = circuit;
        Date = date;
    }

    public override string ToString()
    {
        string result = "Race: + " + RaceName + "\n" +
                        "Season: " + Season + "\n" +
                        "Round: " + Round + "\n" +
                        "Url: " + Url + "\n" +
                        "Circuit: " + Circuit.CircuitId + "\n" + "Date: " + Date.ToString("yyyy-MM-dd");
        return result;
    }
}