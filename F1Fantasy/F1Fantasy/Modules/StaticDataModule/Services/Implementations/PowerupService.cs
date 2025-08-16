using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations;

public class PowerupService : IPowerupService
{
    private readonly IStaticDataRepository _staticDataRepository;
    private readonly WooF1Context _context;

    public PowerupService(IStaticDataRepository staticDataRepository, WooF1Context context)
    {
        _staticDataRepository = staticDataRepository;
        _context = context;
    }
    
    public async Task<PowerupDto> AddPowerupAsync(PowerupDto powerupDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            Powerup existingPowerup = await _staticDataRepository.GetPowerupByTypeAsync(powerupDto.Type);
            if (existingPowerup != null)
            {
                return null;
            }
            
            Powerup powerup = StaticDataDtoMapper.MapDtoToPowerup(powerupDto);

            Powerup newPowerup = await _staticDataRepository.AddPowerupAsync(powerup);

            // Additional operations that need atomicity (example: logging the event)
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return StaticDataDtoMapper.MapPowerupToDto(newPowerup);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating powerup: {ex.Message}");

            throw;
        }
    }
    
    public async void AddListPowerupsAsync(List<PowerupDto> powerupDtos)
    {
        foreach (var powerup in powerupDtos)
        {
            Console.WriteLine($"Adding powerup: {powerup.Type}");
            await AddPowerupAsync(powerup);
        }
    }
    
    public async Task<PowerupDto> GetPowerupByIdAsync(int id)
    {
        Powerup powerup = await _staticDataRepository.GetPowerupByIdAsync(id);
        if (powerup == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapPowerupToDto(powerup);
    }

    public async Task<PowerupDto> GetPowerupByTypeAsync(string type)
    {
        Powerup powerup = await _staticDataRepository.GetPowerupByTypeAsync(type);
        if (powerup == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapPowerupToDto(powerup);
    }
}