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
    public class DriverService(IDataSyncRepository dataSyncRepository, WooF1Context context)
        : IDriverService
    {
        public async Task<DriverDto> AddDriverAsync(DriverDto driverDto)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                Driver existingDriver = await dataSyncRepository.GetDriverByCodeAsync(driverDto.Code);
                if (existingDriver != null)
                {
                    return null;
                }
                
                driverDto = FixSpecialCountryCase(driverDto);
                
                // Driver API returns nationality, so we need check for nationality.
                var country = await dataSyncRepository.GetCountryByNationalitityAsync(driverDto.CountryId);
                if (country == null)
                {
                    throw new Exception($"Country with nationality {driverDto.CountryId} not found");
                }
                driverDto.CountryId = country.Id;

                Driver driver = StaticDataDtoMapper.MapDtoToDriver(driverDto);

                Driver newDriver = await dataSyncRepository.AddDriverAsync(driver);

                // Additional operations that need atomicity (example: logging the event)
                await context.SaveChangesAsync();

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

        public async Task AddListDriversAsync(List<DriverDto> driverDtos)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var exisitngCountries = await dataSyncRepository.GetAllCountriesAsync();
                var existingDrivers = await dataSyncRepository.GetAllDriverCodesAsync();
                var newDrivers = new List<Driver>();
                foreach (var driverDto in driverDtos)
                {
                    if (existingDrivers.Contains(driverDto.Code))
                    {
                        continue;
                    }

                    DriverDto fixedDriverDto = FixSpecialCountryCase(driverDto);
                    // Driver API returns nationality, so we need check for nationality.
                    if (!exisitngCountries.Exists(c => c.Nationalities.Contains(fixedDriverDto.CountryId)))
                    {
                        throw new Exception($"Country with nationality {fixedDriverDto.CountryId} not found");
                    }
                    fixedDriverDto.CountryId = exisitngCountries.First(c => c.Nationalities.Contains(fixedDriverDto.CountryId)).Id;

                    Driver driver = StaticDataDtoMapper.MapDtoToDriver(fixedDriverDto);

                    newDrivers.Add(driver);
                }

                var newDriversReturned = await dataSyncRepository.AddListDriversAsync(newDrivers);
                // Additional operations that need atomicity (example: logging the event)
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating driver: {ex.Message}");

                throw;
            }
        }

        public async Task<DriverDto> GetDriverByIdAsync(int id)
        {
            Driver driver = await dataSyncRepository.GetDriverByIdAsync(id);
            if (driver == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapDriverToDto(driver);
        }

        public async Task<DriverDto> GetDriverByCodeAsync(string code)
        {
            Driver driver = await dataSyncRepository.GetDriverByCodeAsync(code);
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
            return await dataSyncRepository.GetDriversCountAsync();
        }
    }
}