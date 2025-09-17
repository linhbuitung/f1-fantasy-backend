using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;

using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Repositories.Implementations
{
    public class DataSyncRepository(WooF1Context context) : IDataSyncRepository
    {
        public async Task<Driver> AddDriverAsync(Driver driver)
        {
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();
            return driver;
        }

        public async Task<Driver?> GetDriverByIdAsync(int id)
        {
            return await context.Drivers.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Driver?> GetDriverByCodeAsync(string code)
        {
            return await context.Drivers.AsNoTracking().FirstOrDefaultAsync(d => d.Code.Equals(code));
        }

        public async Task<Constructor> AddConstructorAsync(Constructor constructor)
        {
            context.Constructors.Add(constructor);
            await context.SaveChangesAsync();
            return constructor;
        }

        public async Task<Constructor?> GetConstructorByIdAsync(int id)
        {
            return await context.Constructors.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Constructor?> GetConstructorByCodeAsync(string code)
        {
            return await context.Constructors.AsNoTracking().FirstOrDefaultAsync(c => c.Code.Equals(code));
        }

        public async Task<Circuit> AddCircuitAsync(Circuit circuit)
        {
            context.Circuits.Add(circuit);

            await context.SaveChangesAsync();
            return circuit;
        }

        public async Task<Circuit?> GetCircuitByIdAsync(int id)
        {
            return await context.Circuits.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Circuit?> GetCircuitByCodeAsync(string code)
        {
            return await context.Circuits.AsNoTracking().FirstOrDefaultAsync(c => c.Code.Equals(code));
        }

        public async Task<Country> AddCountryAsync(Country country)
        {
            context.Countries.Add(country);

            await context.SaveChangesAsync();
            return country;
        }

        // Country has string Id, so we use Equal method
        public async Task<Country?> GetCountryByIdAsync(string id)
        {
            return await context.Countries.AsNoTracking().FirstOrDefaultAsync(n => n.Id.Equals(id));
        }

        public async Task<Country?> GetCountryByNationalitityAsync(string nationality)
        {
            return await context.Countries.AsNoTracking().FirstOrDefaultAsync(n => n.Nationalities.Contains(nationality));
        }

        public async Task<Country?> GetCountryByShortNameAsync(string shortName)
        {
            return await context.Countries.AsNoTracking().FirstOrDefaultAsync(n => n.ShortName.Equals(shortName));
        }

        public async Task<Race> AddRaceAsync(Race race)
        {
            context.Races.Add(race);

            await context.SaveChangesAsync();
            return race;
        }

        public async Task<Race?> GetRaceByIdAsync(int id)
        {
            return await context.Races.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Race?> GetRaceByRaceDateAsync(DateOnly date)
        {
            return await context.Races.AsNoTracking().FirstOrDefaultAsync(n => n.RaceDate == date);
        }

        public async Task<List<int>> GetAllRaceIdsByYearAsync(int year)
        {
            return await context.Races.AsNoTracking().Where(r => r.Season.Year == year).Select(r => r.Id).ToListAsync();
        }
        public async Task<Powerup> AddPowerupAsync(Powerup powerup)
        {
            context.Powerups.Add(powerup);
            
            await context.SaveChangesAsync();
            return powerup;
        }
        
        public async Task<Powerup?> GetPowerupByIdAsync(int id)
        {
            return await context.Powerups.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }
        
        public async Task<Powerup?> GetPowerupByTypeAsync(string type)
        {
            return await context.Powerups.AsNoTracking().FirstOrDefaultAsync(p => p.Type.Equals(type));
        }
        
        public async Task<List<Powerup>> GetAllPowerupsAsync()
        {
            return await context.Powerups.AsNoTracking().ToListAsync();
        }

        public async Task<Season> AddSeasonAsync(Season season)
        {
            context.Seasons.Add(season);
            
            await context.SaveChangesAsync();
            return season;
        }

        public async Task<Season?> GetSeasonByIdAsync(int id)
        {
            return await context.Seasons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Season?> GetSeasonByYearAsync(int year)
        {
            return await context.Seasons.AsNoTracking().FirstOrDefaultAsync(s => s.Year == year);
        }

        public async Task<RaceEntry> AddRaceEntryAsync(RaceEntry raceEntry)
        {
            context.RaceEntries.Add(raceEntry);
            await context.SaveChangesAsync();
            return raceEntry;
        }

        public async Task<RaceEntry?> GetRaceEntryByIdAsync(int id)
        {
            return await context.RaceEntries.AsNoTracking().FirstOrDefaultAsync(re => re.Id == id);
        }

        public async Task<RaceEntry?> GetRaceEntryByDriverCodeAndCircuitCodeInYearAsync(string driverCode, string circuitCode, int year)
        {
            return await context.RaceEntries.AsNoTracking()
                .FirstOrDefaultAsync(re => re.Race.Season.Year == year && re.Driver.Code.Equals(driverCode) && re.Race.Circuit.Code.Equals(circuitCode));
        }
        
        public async Task<RaceEntry?> GetRaceEntryByDriverIdAndRaceDate(int driverId, DateOnly date)
        {
            return await context.RaceEntries.AsNoTracking().FirstOrDefaultAsync(re => re.Race.RaceDate == date  && re.DriverId == driverId);
        }

        public async Task<List<RaceEntry>> GetRaceEntriesByRaceIdAsync(int raceId)
        {
            return await context.RaceEntries.AsNoTracking().Where(re => re.RaceId == raceId).ToListAsync();
        }
        
        public async Task<FantasyLineup> AddFantasyLineupAsync(FantasyLineup fantasyLineup)
        {
            context.FantasyLineups.Add(fantasyLineup);
            await context.SaveChangesAsync();
            return fantasyLineup;
        }

        public async Task<FantasyLineup?> GetFantasyLineupByUserIdAndRaceId(int userId, int raceId)
        {
            return await context.FantasyLineups.AsNoTracking().FirstOrDefaultAsync(f => f.UserId.Equals(userId) && f.RaceId.Equals(raceId));
        }
        
        public async Task<AspNetUser> GetUserByIdAsync(int id)
        {
            return await context.AspNetUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<int>> GetAllUserIdsAsync()
        {
            return await context.AspNetUsers.Select(u => u.Id).ToListAsync();
        }

        public async Task<PickableItem?> GetPickableItemAsync()
        {
            return await context.PickableItems.FirstOrDefaultAsync(p => p.Id == 1);
        }

        public async Task<PickableItem> AddPickableItemAsync()
        {
            var item = new PickableItem{Id = 1};
            context.PickableItems.Add(item);
            await context.SaveChangesAsync();
            return item;
        }
        #region GetCounts

        public async Task<int> GetDriversCountAsync()
        {
            return await context.Drivers.CountAsync();
        }

        public async Task<int> GetConstructorsCountAsync()
        {
            return await context.Constructors.CountAsync();
        }

        public async Task<int> GetCircuitsCountAsync()
        {
            return await context.Circuits.CountAsync();
        }

        public async Task<int> GetRacesCountAsync()
        {
            return await context.Races.CountAsync();
        }

        public async Task<int> GetSeasonsCountAsync()
        {
            return await context.Seasons.CountAsync();
        }

        public async Task<int> GetRaceEntriesCountAsync()
        {
            return await context.RaceEntries.CountAsync();
        }
        #endregion
    }
}