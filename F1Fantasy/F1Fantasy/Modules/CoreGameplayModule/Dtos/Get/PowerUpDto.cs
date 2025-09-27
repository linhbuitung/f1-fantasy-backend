namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

public class PowerupDto
{
    public int? Id { get; set; } 

    public string Type { get; set; }  = null!;

    public string Description { get; set; } = null!;

    public string ImgUrl { get; set; }  = null!;
}