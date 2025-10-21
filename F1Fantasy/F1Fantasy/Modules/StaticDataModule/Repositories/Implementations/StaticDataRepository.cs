using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;

using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace F1Fantasy.Modules.StaticDataModule.Repositories.Implementations
{
    public class StaticDataRepository(WooF1Context context) : IStaticDataRepository
    {
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await context.Countries.AsNoTracking().ToListAsync();
        }
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
        public async Task<Driver?> GetDriverByIdAsTrackingAsync(int id)
        {
            return await context.Drivers.AsTracking().FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<Driver?> GetDriverByCodeAsync(string code)
        {
            return await context.Drivers.AsNoTracking().FirstOrDefaultAsync(d => d.Code.Equals(code));
        }

        public async Task<List<Driver>> GetAllDriversAsync()
        {
            return await context.Drivers.AsNoTracking().ToListAsync();
        }
        public async Task<List<Driver>> GetAllDriversBySeasonIdAsync(int seasonId)
        {
            return await context.Drivers.AsNoTracking()
                .Where(d => d.RaceEntries.Any(re => re.Race.SeasonId == seasonId))
                .ToListAsync();  
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
        public async Task<Constructor?> GetConstructorByIdAsTrackingAsync(int id)
        {
            return await context.Constructors.AsTracking().FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Constructor?> GetConstructorByCodeAsync(string code)
        {
            return await context.Constructors.AsNoTracking().FirstOrDefaultAsync(c => c.Code.Equals(code));
        }
        
        public async Task<List<Constructor>> GetAllConstructorsAsync()
        {
            return await context.Constructors.AsNoTracking().ToListAsync();
        }
        
        public async Task<List<Constructor>> GetAllConstructorsBySeasonIdAsync(int seasonId)
        {
            return await context.Constructors.AsNoTracking()
                .Where(d => d.RaceEntries.Any(re => re.Race.SeasonId == seasonId))
                .ToListAsync();  
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

        public async Task<List<Circuit>> GetAllCircuitsAsync()
        {
            return await context.Circuits.AsNoTracking().ToListAsync();
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

        public async Task<Country?> GetCountryByNationalityAsync(string nationality)
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
            return await context.Races.Include(r => r.Season)
                .Include(r => r.Circuit)
                .Include(r => r.RaceEntries)
                .ThenInclude(r => r.Driver)
                .Include(r => r.RaceEntries)
                .ThenInclude(r => r.Constructor)
                .AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Race?> GetRaceByRaceDateAsync(DateOnly date)
        {
            return await context.Races.AsNoTracking().FirstOrDefaultAsync(n => n.RaceDate == date);
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
    }
}