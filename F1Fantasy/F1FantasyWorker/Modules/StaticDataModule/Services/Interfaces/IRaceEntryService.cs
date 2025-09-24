using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

public interface IRaceEntryService
{
    Task<RaceEntryDto> AddRaceEntryAsync(RaceEntryDto raceEntryDto);
    
    Task AddListRaceEntriesAsync(List<RaceEntryDto> raceEntryDtos);
    
    Task<RaceEntryDto> GetRaceEntryByIdAsync(int id);
    
    Task<int> GetRaceEntriesCountAsync();
}