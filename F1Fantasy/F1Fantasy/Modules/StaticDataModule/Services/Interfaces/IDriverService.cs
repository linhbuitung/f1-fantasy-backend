using F1Fantasy.Modules.StaticDataModule.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces
{
    public interface IDriverService
    {
        //IEnumerable<DriverDto> GetAllDrivers();
        //DriverDto GetDriverById(int id);
        Task<DriverDto> AddDriverAsync(DriverDto driverDto);

        void AddListDriversAsync(List<DriverDto> driverDtos);

        //void UpdateDriver(DriverDto driver);
        // void DeleteDriver(int id);
    }
}