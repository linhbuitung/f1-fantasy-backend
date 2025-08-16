using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces
{
    public interface IStaticDataRepository
    {
        //IEnumerable<DriverDto> GetAllDrivers();
        //DriverDto GetDriverById(int id);
        Task<Driver> AddDriverAsync(Driver driver);

        Task<Driver> GetDriverByIdAsync(int id);

        Task<Driver> GetDriverByCodeAsync(string code);

        Task<Constructor> AddConstructorAsync(Constructor constructor);

        Task<Constructor> GetConstructorByIdAsync(int id);

        Task<Constructor> GetConstructorByCodeAsync(string code);

        Task<Circuit> AddCircuitAsync(Circuit circuit);

        Task<Circuit> GetCircuitByIdAsync(int id);

        Task<Circuit> GetCircuitByCodeAsync(string code);

        Task<Country> AddCountryAsync(Country country);

        Task<Country> GetCountryByIdAsync(string id);

        Task<Country> GetCountryByNationalitityAsync(string nationality);

        Task<Country> GetCountryByShortNameAsync(string shortName);
        Task<Race> AddRaceAsync(Race race);

        Task<Race> GetRaceByIdAsync(int id);

        Task<Race> GetRaceByRaceDateAsync(DateOnly date);

        Task<Powerup> AddPowerupAsync(Powerup powerup);

        Task<Powerup> GetPowerupByIdAsync(int id);

        Task<Powerup> GetPowerupByTypeAsync(string type);


        //void UpdateDriver(DriverDto driver);
        // void DeleteDriver(int id);
    }
}