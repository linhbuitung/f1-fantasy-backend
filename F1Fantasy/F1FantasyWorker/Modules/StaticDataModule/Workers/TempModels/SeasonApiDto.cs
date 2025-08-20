namespace F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

public class SeasonApiDto
{
    public int Season { get; set; }
    
    public string Url { get; set; }
    
    public SeasonApiDto(int season, string url)
    {
        Season = season;
        Url = url;
    }

    public override string ToString()
    {
        string result = $"Season Info: Season={Season}, Url={Url}";
        return result;
    }
}