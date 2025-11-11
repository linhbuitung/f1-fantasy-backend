using F1Fantasy.Modules.StaticDataModule.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

public interface IRaceService
{
    Task<RaceDto> AddRaceAsync(RaceDto raceDto);

    void AddListRacesAsync(List<RaceDto> raceDtos);

    Task<RaceDto> GetRaceByIdAsync(int id);

    Task<RaceDto> GetRaceByRaceDateAsync(DateOnly date);
    
    Task<List<RaceDto>> GetRacesBySeasonIdAsync(int seasonId);
}