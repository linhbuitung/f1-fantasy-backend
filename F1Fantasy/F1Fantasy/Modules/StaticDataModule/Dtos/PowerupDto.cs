namespace F1Fantasy.Modules.StaticDataModule.Dtos;

public class PowerupDto
{
    public int? Id { get; set; }
    
    public string Type { get; set; }
    
    public string Description { get; set; }
    
    public string ImgUrl { get; set; }
    
    public PowerupDto(int? id, string type, string description, string imgUrl)
    {
        Id = id ?? null;
        Type = type;
        Description = description;
        ImgUrl = imgUrl;
    }
}