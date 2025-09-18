namespace F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

public class RaceApiDto(int season, int round, string url, string raceName, CircuitApiDto circuit, DateOnly date)
{
    public int Season { get; set; } = season;

    public int Round { get; set; } = round;

    public string Url { get; set; } = url;

    public string RaceName { get; set; } = raceName;

    public CircuitApiDto Circuit { get; set; } = circuit;

    public DateOnly Date  { get; set; } = date;

    public List<RaceEntryApiDto>? Results { get; set; }

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