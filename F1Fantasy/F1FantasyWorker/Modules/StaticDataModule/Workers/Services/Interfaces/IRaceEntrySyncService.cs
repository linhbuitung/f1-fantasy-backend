using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;

public interface IRaceEntrySyncService
{
    Task<List<RaceEntryApiDto>> GetRaceEntriesForCurrentYearFromApiAsync();
    Task<List<RaceEntryDto>> GetStaticRaceEntriesAsync();
}