namespace F1FantasyWorker.Modules.StaticDataModule.Dtos;

public class SeasonDto(int? id, int year, bool isActive)
{
    public int? Id { get; set; } = id ?? null;

    public int Year { get; set; } = year;

    public bool IsActive { get; set; } = isActive;
}