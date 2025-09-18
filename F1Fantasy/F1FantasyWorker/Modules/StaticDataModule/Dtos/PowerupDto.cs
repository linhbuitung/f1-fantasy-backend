namespace F1FantasyWorker.Modules.StaticDataModule.Dtos;

public class PowerupDto(int? id, string type, string description, string imgUrl)
{
    public int? Id { get; set; } = id ?? null;

    public string Type { get; set; } = type;

    public string Description { get; set; } = description;

    public string ImgUrl { get; set; } = imgUrl;
}