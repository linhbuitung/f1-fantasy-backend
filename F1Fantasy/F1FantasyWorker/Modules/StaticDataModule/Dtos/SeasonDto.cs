namespace F1FantasyWorker.Modules.StaticDataModule.Dtos;

public class SeasonDto
{
    public int? Id { get; set; }

    public int Year { get; set; }

    public bool IsActive { get; set; }
    
    public SeasonDto(int? id, int year, bool isActive)
    {
        Id = id ?? null;
        Year = year;
        IsActive = isActive;
    }
}