using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations;

public class PowerupService(IStaticDataRepository staticDataRepository, WooF1Context context)
    : IPowerupService
{
    public async Task<PowerupDto> AddPowerupAsync(PowerupDto powerupDto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            Powerup? existingPowerup = await staticDataRepository.GetPowerupByTypeAsync(powerupDto.Type);
            if (existingPowerup != null)
            {
                return null;
            }
            
            Powerup powerup = StaticDataDtoMapper.MapDtoToPowerup(powerupDto);

            Powerup newPowerup = await staticDataRepository.AddPowerupAsync(powerup);

            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();

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
        Powerup? powerup = await staticDataRepository.GetPowerupByIdAsync(id);
        if (powerup == null)
        {
            throw new NotFoundException($"Powerup with id {id} not found");
        }
        return StaticDataDtoMapper.MapPowerupToDto(powerup);
    }

    public async Task<PowerupDto> GetPowerupByTypeAsync(string type)
    {
        Powerup? powerup = await staticDataRepository.GetPowerupByTypeAsync(type);
        if (powerup == null)
        {
            throw new NotFoundException($"Powerup with type {type} not found");
        }
        return StaticDataDtoMapper.MapPowerupToDto(powerup);
    }

    public async Task<List<PowerupDto>> GetAllPowerupsAsync()
    {
        List<Powerup> powerups = await staticDataRepository.GetAllPowerupsAsync();
        return powerups.Select(StaticDataDtoMapper.MapPowerupToDto).ToList();
    }
}