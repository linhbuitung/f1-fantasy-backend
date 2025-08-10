using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;

using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace F1Fantasy.Modules.StaticDataModule.Repositories.Implementations
{
    public class StaticDataRepository : IStaticDataRepository
    {
        private readonly WooF1Context _context;

        public StaticDataRepository(WooF1Context context)
        {
            _context = context;
        }

        public async Task<Driver> AddDriverAsync(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return driver;
        }

        public async Task<Driver> GetDriverByIdAsync(int id)
        {
            return await _context.Drivers.Include(d => d.Country).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Driver> GetDriverByCodeAsync(string code)
        {
            return await _context.Drivers.Include(d => d.Country).FirstOrDefaultAsync(d => d.Code.Equals(code));
        }

        public async Task<Constructor> AddConstructorAsync(Constructor constructor)
        {
            _context.Constructors.Add(constructor);
            await _context.SaveChangesAsync();
            return constructor;
        }

        public async Task<Constructor> GetConstructorByIdAsync(int id)
        {
            return await _context.Constructors.Include(d => d.Country).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Constructor> GetConstructorByCodeAsync(string code)
        {
            return await _context.Constructors.Include(d => d.Country).FirstOrDefaultAsync(c => c.Code.Equals(code));
        }

        public async Task<Circuit> AddCircuitAsync(Circuit circuit)
        {
            _context.Circuits.Add(circuit);

            await _context.SaveChangesAsync();
            return circuit;
        }

        public async Task<Circuit> GetCircuitByIdAsync(int id)
        {
            return await _context.Circuits.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Circuit> GetCircuitByCodeAsync(string code)
        {
            return await _context.Circuits.FirstOrDefaultAsync(c => c.Code.Equals(code));
        }

        public async Task<Country> AddNationalityAsync(Country nationality)
        {
            _context.Nationalities.Add(nationality);
            await _context.SaveChangesAsync();
            return nationality;
        }

        public async Task<Country> GetNationalityByIdAsync(string NationalityId)
        {
            return await _context.Nationalities.FirstOrDefaultAsync(n => n.Id.Equals(NationalityId));
        }

        public async Task<IEnumerable<Country>> GetAllNationalitiesAsync()
        {
            return await _context.Nationalities.ToListAsync();
        }
    }
}