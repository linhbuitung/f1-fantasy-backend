using F1FantasyWorker.Core.Common;

namespace F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces
{
    public interface IDataSyncRepository
    {
        //IEnumerable<DriverDto> GetAllDrivers();
        //DriverDto GetDriverById(int id);
        Task<Driver> AddDriverAsync(Driver driver);

        Task<List<Driver>> AddListDriversAsync(List<Driver> drivers);
        Task<List<string>> GetAllDriverCodesAsync();
        Task<Driver?> GetDriverByIdAsync(int id);

        Task<Driver?> GetDriverByCodeAsync(string code);
        
        Task<List<Driver>> GetAllDriversAsync();

        Task<Constructor> AddConstructorAsync(Constructor constructor);

        Task<List<Constructor>> AddListConstructorsAsync(List<Constructor> constructors);
        Task<List<string>> GetAllConstructorCodesAsync();
        Task<Constructor?> GetConstructorByIdAsync(int id);

        Task<Constructor?> GetConstructorByCodeAsync(string code);

        Task<List<Constructor>> GetAllConstructorsAsync();

        Task<Circuit> AddCircuitAsync(Circuit circuit);
        
        Task<List<Circuit>> AddListCircuitsAsync(List<Circuit> circuits);
        Task<List<string>> GetAllCircuitCodesAsync();

        Task<Circuit?> GetCircuitByIdAsync(int id);

        Task<Circuit?> GetCircuitByCodeAsync(string code);

        Task<List<Circuit>> GetAllCircuitsAsync();
        Task<Country> AddCountryAsync(Country country);
        
        Task<List<Country>> AddListCountriesAsync(List<Country> countries);

        Task<Country?> GetCountryByIdAsync(string id);

        Task<List<Country>> GetAllCountriesAsync();

        Task<Country?> GetCountryByNationalitityAsync(string nationality);

        Task<Country?> GetCountryByShortNameAsync(string shortName);
        Task<Race> AddRaceAsync(Race race);
        
        Task<List<Race>> AddListRacesAsync(List<Race> races);
        Task<List<DateOnly>> GetAllRaceDatesAsync();

        Task<Race?> GetRaceByIdAsync(int id);

        Task<Race?> GetRaceByRaceDateAsync(DateOnly date);
        
        Task<List<int>> GetAllRaceIdsByYearAsync(int year);

        Task<List<Race>> GetAllRacesAsync();
        Task<Powerup> AddPowerupAsync(Powerup powerup);

        Task<List<Powerup>> AddListPowerupAsync(List<Powerup> powerups);

        Task<Powerup?> GetPowerupByIdAsync(int id);

        Task<Powerup?> GetPowerupByTypeAsync(string type);

        Task<List<Powerup>> GetAllPowerupsAsync();

        Task<Season> AddSeasonAsync(Season season);
        
        Task<List<Season>> AddListSeasonsAsync(List<Season> seasons);
        Task<List<int>> GetAllSeasonYearsAsync();

        Task<Season?> GetSeasonByIdAsync(int id);

        Task<Season?> GetSeasonByYearAsync(int year);
        
        Task<List<Season>> GetAllSeasonsAsync();
        Task<RaceEntry> AddRaceEntryAsync(RaceEntry raceEntry);

        Task<List<RaceEntry>> AddListRaceEntriesAsync(List<RaceEntry> raceEntries);
        Task<List<RaceEntry>> GetAllRaceEntriesBySeasonYearAsync(int year);
        Task<RaceEntry?> GetRaceEntryByIdAsync(int id);

        Task<RaceEntry?> GetRaceEntryByDriverCodeAndCircuitCodeInYearAsync(string driverCode, string circuitCode, int year);

        Task<RaceEntry?> GetRaceEntryByDriverIdAndRaceDate(int driverId, DateOnly date);

        Task<List<RaceEntry>> GetRaceEntriesByRaceIdAsync(int raceId);
        
        Task<FantasyLineup> AddFantasyLineupAsync(FantasyLineup fantasyLineup);
        
        Task<List<FantasyLineup>> AddListFantasyLineupsAsync(List<FantasyLineup> fantasyLineups);
        
        Task<FantasyLineup?> GetFantasyLineupByUserIdAndRaceId(int userId, int raceId);
        
        Task<List<FantasyLineup>> GetAllFantasyLineupsInSeasonYearAsync(int year);
        
        Task<AspNetUser> GetUserByIdAsync(int id);
        
        Task<List<int>> GetAllUserIdsAsync();

        Task<PickableItem?> GetPickableItemAsync();
        
        Task<PickableItem> AddPickableItemAsync();
        #region GetCounts

        Task<int> GetDriversCountAsync();
        
        Task<int> GetConstructorsCountAsync();
        
        Task<int> GetCircuitsCountAsync();
        Task<int> GetRacesCountAsync();
        
        Task<int> GetSeasonsCountAsync();
        
        Task<int> GetRaceEntriesCountAsync();

        #endregion
    }
}