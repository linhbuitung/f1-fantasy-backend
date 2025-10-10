using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces
{
    public interface IStaticDataRepository
    {
        //IEnumerable<DriverDto> GetAllDrivers();
        //DriverDto GetDriverById(int id);
        Task<List<Country>> GetAllCountriesAsync();
        Task<Driver> AddDriverAsync(Driver driver);

        Task<Driver?> GetDriverByIdAsync(int id);
        Task<Driver?> GetDriverByIdAsTrackingAsync(int id);
        Task<Driver?> GetDriverByCodeAsync(string code);
        
        Task<List<Driver>> GetAllDriversAsync();
        
        Task<List<Driver>> GetAllDriversBySeasonIdAsync(int seasonId);

        Task<Constructor> AddConstructorAsync(Constructor constructor);

        Task<Constructor?> GetConstructorByIdAsync(int id);

        Task<Constructor?> GetConstructorByIdAsTrackingAsync(int id);

        Task<Constructor?> GetConstructorByCodeAsync(string code);
        
        Task<List<Constructor>> GetAllConstructorsAsync();
        Task<List<Constructor>> GetAllConstructorsBySeasonIdAsync(int seasonId);

        Task<Circuit> AddCircuitAsync(Circuit circuit);

        Task<Circuit?> GetCircuitByIdAsync(int id);

        Task<Circuit?> GetCircuitByCodeAsync(string code);

        Task<Country> AddCountryAsync(Country country);

        Task<Country?> GetCountryByIdAsync(string id);

        Task<Country?> GetCountryByNationalityAsync(string nationality);

        Task<Country?> GetCountryByShortNameAsync(string shortName);
        Task<Race> AddRaceAsync(Race race);

        Task<Race?> GetRaceByIdAsync(int id);

        Task<Race?> GetRaceByRaceDateAsync(DateOnly date);

        Task<Powerup> AddPowerupAsync(Powerup powerup);

        Task<Powerup?> GetPowerupByIdAsync(int id);

        Task<Powerup?> GetPowerupByTypeAsync(string type);
        
        Task<List<Powerup>> GetAllPowerupsAsync();

        Task<Season> AddSeasonAsync(Season season);

        Task<Season?> GetSeasonByIdAsync(int id);

        Task<Season?> GetSeasonByYearAsync(int year);
        //void UpdateDriver(DriverDto driver);
        // void DeleteDriver(int id);
    }
}