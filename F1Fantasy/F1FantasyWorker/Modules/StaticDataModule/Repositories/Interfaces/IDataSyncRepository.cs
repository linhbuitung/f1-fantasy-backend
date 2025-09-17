using F1FantasyWorker.Core.Common;

namespace F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces
{
    public interface IDataSyncRepository
    {
        //IEnumerable<DriverDto> GetAllDrivers();
        //DriverDto GetDriverById(int id);
        Task<Driver> AddDriverAsync(Driver driver);

        Task<Driver?> GetDriverByIdAsync(int id);

        Task<Driver?> GetDriverByCodeAsync(string code);

        Task<Constructor> AddConstructorAsync(Constructor constructor);

        Task<Constructor?> GetConstructorByIdAsync(int id);

        Task<Constructor?> GetConstructorByCodeAsync(string code);

        Task<Circuit> AddCircuitAsync(Circuit circuit);

        Task<Circuit?> GetCircuitByIdAsync(int id);

        Task<Circuit?> GetCircuitByCodeAsync(string code);

        Task<Country> AddCountryAsync(Country country);

        Task<Country?> GetCountryByIdAsync(string id);

        Task<Country?> GetCountryByNationalitityAsync(string nationality);

        Task<Country?> GetCountryByShortNameAsync(string shortName);
        Task<Race> AddRaceAsync(Race race);

        Task<Race?> GetRaceByIdAsync(int id);

        Task<Race?> GetRaceByRaceDateAsync(DateOnly date);
        
        Task<List<int>> GetAllRaceIdsByYearAsync(int year);

        Task<Powerup> AddPowerupAsync(Powerup powerup);

        Task<Powerup?> GetPowerupByIdAsync(int id);

        Task<Powerup?> GetPowerupByTypeAsync(string type);

        Task<List<Powerup>> GetAllPowerupsAsync();

        Task<Season> AddSeasonAsync(Season season);

        Task<Season?> GetSeasonByIdAsync(int id);

        Task<Season?> GetSeasonByYearAsync(int year);
        
        Task<RaceEntry> AddRaceEntryAsync(RaceEntry raceEntry);

        Task<RaceEntry?> GetRaceEntryByIdAsync(int id);

        Task<RaceEntry?> GetRaceEntryByDriverCodeAndCircuitCodeInYearAsync(string driverCode, string circuitCode, int year);

        Task<RaceEntry?> GetRaceEntryByDriverIdAndRaceDate(int driverId, DateOnly date);

        Task<List<RaceEntry>> GetRaceEntriesByRaceIdAsync(int raceId);
        
        Task<FantasyLineup> AddFantasyLineupAsync(FantasyLineup fantasyLineup);
        
        Task<FantasyLineup?> GetFantasyLineupByUserIdAndRaceId(int userId, int raceId);
        
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