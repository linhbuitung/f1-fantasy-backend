using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using F1FantasyWorker.Modules.StaticDataModule.Configs;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations
{
    public class DriverService : IDriverService
    {
        private readonly IDataSyncRepository _dataSyncRepository;
        private readonly WooF1Context _context;

        public DriverService(IDataSyncRepository dataSyncRepository, WooF1Context context)
        {
            _dataSyncRepository = dataSyncRepository;
            _context = context;
        }

        public async Task<DriverDto> AddDriverAsync(DriverDto driverDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Driver existingDriver = await _dataSyncRepository.GetDriverByCodeAsync(driverDto.Code);
                if (existingDriver != null)
                {
                    return null;
                }
                
                driverDto = FixSpecialCountryCase(driverDto);
                
                // Driver API returns nationality, so we need check for nationality.
                Country country = await _dataSyncRepository.GetCountryByNationalitityAsync(driverDto.CountryId);
                if (country == null)
                {
                    throw new Exception($"Country with nationality {driverDto.CountryId} not found");
                }
                driverDto.CountryId = country.Id;

                Driver driver = StaticDataDtoMapper.MapDtoToDriver(driverDto);

                Driver newDriver = await _dataSyncRepository.AddDriverAsync(driver);

                // Additional operations that need atomicity (example: logging the event)
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StaticDataDtoMapper.MapDriverToDto(newDriver);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating driver: {ex.Message}");

                throw;
            }
        }

        public async void AddListDriversAsync(List<DriverDto> driverDtos)
        {
            foreach (var driver in driverDtos)
            {
                Console.WriteLine($"Adding driver: {driver.FamilyName} with code: {driver.Code}");
                await AddDriverAsync(driver);
            }
        }

        public async Task<DriverDto> GetDriverByIdAsync(int id)
        {
            Driver driver = await _dataSyncRepository.GetDriverByIdAsync(id);
            if (driver == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapDriverToDto(driver);
        }

        public async Task<DriverDto> GetDriverByCodeAsync(string code)
        {
            Driver driver = await _dataSyncRepository.GetDriverByCodeAsync(code);
            if (driver == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapDriverToDto(driver);
        }

        private DriverDto FixSpecialCountryCase(DriverDto driverDto)
        {
            if (driverDto.CountryId.Equals("East German"))
            {
                driverDto.CountryId = "German";
            }
            else if (driverDto.CountryId.Equals("Rhodesian"))
            {
                driverDto.CountryId = "Zimbabwean";
            }
            

            return driverDto;
        }
        
        public async Task<int> GetDriversCountAsync()
        {
            return await _dataSyncRepository.GetDriversCountAsync();
        }
    }
}