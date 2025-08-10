using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces
{
    public interface IDriverService
    {
        //IEnumerable<DriverDto> GetAllDrivers();
        //DriverDto GetDriverById(int id);
        Task<DriverDto> AddDriverAsync(DriverDto driverDto);

        void AddListDriversAsync(List<DriverDto> driverDtos);

        Task<DriverDto> GetDriverByIdAsync(int id);

        Task<DriverDto> GetDriverByCodeAsync(string code);

        //void UpdateDriver(DriverDto driver);

        // void DeleteDriver(int id);
    }
}