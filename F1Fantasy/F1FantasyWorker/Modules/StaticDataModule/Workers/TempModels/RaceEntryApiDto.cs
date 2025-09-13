namespace F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

public class RaceEntryApiDto
{
    public int Position { get; set; }
    public int Grid { get; set; }
    
    public string Status { get; set; }
    public FastestLap? FastestLap { get; set; }
    public DriverApiDto Driver { get; set; }
    public ConstructorApiDto Constructor { get; set; }
    
    public DateOnly RaceDate { get; set; }
}

public class FastestLap
{
    public int Rank { get; set; }
    public string Lap { get; set; }
}