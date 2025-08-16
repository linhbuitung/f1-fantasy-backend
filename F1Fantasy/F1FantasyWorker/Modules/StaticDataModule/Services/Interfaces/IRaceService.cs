using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

public interface IRaceService
{
    Task<RaceDto> AddRaceAsync(RaceDto raceDto);

    void AddListRacesAsync(List<RaceDto> raceDtos);

    Task<RaceDto> GetRaceByIdAsync(int id);

    Task<RaceDto> GetRaceByRaceDateAsync(DateOnly date);
}