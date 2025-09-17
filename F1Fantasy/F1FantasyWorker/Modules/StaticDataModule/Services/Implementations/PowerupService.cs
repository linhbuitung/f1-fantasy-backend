using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class PowerupService(IDataSyncRepository dataSyncRepository, WooF1Context context)
    : IPowerupService
{
    public async Task<PowerupDto> AddPowerupAsync(PowerupDto powerupDto)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            Powerup existingPowerup = await dataSyncRepository.GetPowerupByTypeAsync(powerupDto.Type);
            if (existingPowerup != null)
            {
                return null;
            }
            
            Powerup powerup = StaticDataDtoMapper.MapDtoToPowerup(powerupDto);

            Powerup newPowerup = await dataSyncRepository.AddPowerupAsync(powerup);

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
        Powerup powerup = await dataSyncRepository.GetPowerupByIdAsync(id);
        if (powerup == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapPowerupToDto(powerup);
    }

    public async Task<PowerupDto> GetPowerupByTypeAsync(string type)
    {
        Powerup powerup = await dataSyncRepository.GetPowerupByTypeAsync(type);
        if (powerup == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapPowerupToDto(powerup);
    }
}