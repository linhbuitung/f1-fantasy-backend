using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;

public interface IPowerupSyncService
{
    Task<List<PowerupDto>> GetPowerupsFromStaticResourcesAsync();
}