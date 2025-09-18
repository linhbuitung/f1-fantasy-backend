namespace F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

public class SeasonApiDto(int season, string url)
{
    public int Season { get; set; } = season;

    public string Url { get; set; } = url;

    public override string ToString()
    {
        string result = $"Season Info: Season={Season}, Url={Url}";
        return result;
    }
}