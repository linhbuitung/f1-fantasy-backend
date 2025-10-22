namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class DriverStatisticBaseGetDto
{
    public int Id { get; set; }
    
    public string GivenName { get; set; }
    
    public string FamilyName { get; set; }
    
    public string Code { get; set; }
        
    public int Price { get; set; }

    public string? ImgUrl { get; set; }
}