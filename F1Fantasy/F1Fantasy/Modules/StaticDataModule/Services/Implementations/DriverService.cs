using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using F1Fantasy.Exceptions;
using F1Fantasy.Modules.LeagueModule.Dtos.Mapper;
using F1Fantasy.Shared.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations
{
    public class DriverService(IStaticDataRepository staticDataRepository, IStaticDataSearchRepository searchRepository, WooF1Context context)
        : IDriverService
    {
        public async Task<DriverDto> AddDriverAsync(DriverDto driverDto)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                Driver? existingDriver = await staticDataRepository.GetDriverByCodeAsync(driverDto.Code);
                if (existingDriver != null)
                {
                    return null;
                }
                
                driverDto = FixSpecialCountryCase(driverDto);
                
                // Driver API returns nationality, so we need check for nationality.
                Country? country = await staticDataRepository.GetCountryByNationalityAsync(driverDto.CountryId);
                if (country == null)
                {
                    throw new NotFoundException($"Country with nationality {driverDto.CountryId} not found");
                }
                driverDto.CountryId = country.Id;

                Driver driver = StaticDataDtoMapper.MapDtoToDriver(driverDto);

                Driver newDriver = await staticDataRepository.AddDriverAsync(driver);

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
            Driver? driver = await staticDataRepository.GetDriverByIdAsync(id);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with id {id} not found");
            }
            return StaticDataDtoMapper.MapDriverToDto(driver);
        }

        public async Task<DriverDto> GetDriverByCodeAsync(string code)
        {
            Driver? driver = await staticDataRepository.GetDriverByCodeAsync(code);
            if (driver == null)
            {
                throw new NotFoundException($"Driver with code {code} not found");
            }
            return StaticDataDtoMapper.MapDriverToDto(driver);
        }

        public async Task<List<DriverDto>> GetAllDriversAsync()
        {
            List<Driver> drivers = await staticDataRepository.GetAllDriversAsync();
            return drivers.Select(StaticDataDtoMapper.MapDriverToDto).ToList();
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
        
        public async Task<PagedResult<DriverDto>> SearchDriversAsync(string query, int pageNum, int pageSize)
        {
            int skip = (pageNum - 1) * pageSize;
            var (drivers, totalCount) = await searchRepository.SearchDriversAsync(query, skip, pageSize);

            var items = drivers.Select(StaticDataDtoMapper.MapDriverToDto).ToList();

            return new PagedResult<DriverDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNum = pageNum,
                PageSize = pageSize
            };
        }
    }
}