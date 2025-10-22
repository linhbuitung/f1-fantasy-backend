namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class DriverWIthFantasyPointScored
{
    public int Id { get; set; }
    
    public string GivenName { get; set; }
    
    public string FamilyName { get; set; }
    
    public int FantasyPointScored { get; set; }
    
    public string Code { get; set; }
        
    public int Price { get; set; }

    public string? ImgUrl { get; set; }
}