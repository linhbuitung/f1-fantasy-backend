using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations
{
    public class DriverService : IDriverService
    {
        private readonly IStaticDataRepository _staticDataRepository;
        private readonly WooF1Context _context;

        public DriverService(IStaticDataRepository staticDataRepository, WooF1Context context)
        {
            _staticDataRepository = staticDataRepository;
            _context = context;
        }

        public async Task<DriverDto> AddDriverAsync(DriverDto driverDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Driver existingDriver = await _staticDataRepository.GetDriverByCodeAsync(driverDto.Code);
                if (existingDriver != null)
                {
                    return null;
                }

                Driver driver = StaticDataDtoMapper.MapDtoToDriver(driverDto);

                Driver newDriver = await _staticDataRepository.AddDriverAsync(driver);

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

        //get
        public async Task<DriverDto> GetDriverByIdAsync(Guid id)
        {
            Driver driver = await _staticDataRepository.GetDriverByIdAsync(id);
            if (driver == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapDriverToDto(driver);
        }

        //get by code
        public async Task<DriverDto> GetDriverByCodeAsync(string code)
        {
            Driver driver = await _staticDataRepository.GetDriverByCodeAsync(code);
            if (driver == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapDriverToDto(driver);
        }
    }
}