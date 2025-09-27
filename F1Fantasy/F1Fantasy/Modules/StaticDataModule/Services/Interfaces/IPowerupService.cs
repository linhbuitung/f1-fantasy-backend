using F1Fantasy.Modules.StaticDataModule.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

public interface IPowerupService
{
    Task<PowerupDto> AddPowerupAsync(PowerupDto powerupDto);

    void AddListPowerupsAsync(List<PowerupDto> powerupDto);

    Task<PowerupDto> GetPowerupByIdAsync(int id);

    Task<PowerupDto> GetPowerupByTypeAsync(string type);
    
    Task<List<PowerupDto>> GetAllPowerupsAsync();
}