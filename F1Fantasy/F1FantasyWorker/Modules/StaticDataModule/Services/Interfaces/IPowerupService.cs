using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

public interface IPowerupService
{
    Task<PowerupDto> AddPowerupAsync(PowerupDto powerupDto);

    void AddListPowerupsAsync(List<PowerupDto> powerupDto);

    Task<PowerupDto> GetPowerupByIdAsync(int id);

    Task<PowerupDto> GetPowerupByTypeAsync(string type);
}