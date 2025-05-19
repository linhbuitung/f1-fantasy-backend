using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations
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
    }
}