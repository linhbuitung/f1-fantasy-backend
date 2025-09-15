using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;

public interface IRaceSyncService
{
    Task<List<RaceApiDto>> GetRacesFromApiAsync();
}