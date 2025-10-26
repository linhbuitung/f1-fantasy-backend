using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Shared.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces
{
    public interface IDriverService
    {
        Task<DriverDto> AddDriverAsync(DriverDto driverDto);

        void AddListDriversAsync(List<DriverDto> driverDtos);

        Task<DriverDto> GetDriverByIdAsync(int id);

        Task<DriverDto> GetDriverByCodeAsync(string code);
        
        Task<List<DriverDto>> GetAllDriversAsync();
        
        Task<PagedResult<DriverDto>> SearchDriversAsync(string query, int pageNum, int pageSize);

    }
}