using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;
using F1Fantasy.Modules.CoreGameplayModule.Dtos.Mapper;
using F1Fantasy.Modules.CoreGameplayModule.Repositories.Interfaces;
using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

namespace F1Fantasy.Modules.CoreGameplayModule.Services.Implementations;

public class CoreGameplayService(IStaticDataRepository staticDataRepository, IFantasyLineupRepository fantasyLineupRepository, ICoreGameplayRepository coreGameplayRepository, WooF1Context context, IConfiguration configuration) : ICoreGameplayService
{
    public async Task<Dtos.Get.FantasyLineupDto> GetFantasyLineupByIdAsync(int fantasyLineupId)
    {
        var fantasyLineup = await fantasyLineupRepository.GetFantasyLineupByIdAsync(fantasyLineupId);
        if (fantasyLineup == null)
        {
            throw new NotFoundException("Fantasy lineup not found");
        }
        return CoreGameplayDtoMapper.MapFantasyLineupToGetDto(fantasyLineup);
    }

    public async Task<Dtos.Get.FantasyLineupDto> GetFantasyLineupByUserIdAndRaceIdAsync(int userId, int raceId)
    {
        var fantasyLineup = await fantasyLineupRepository.GetFantasyLineupByUserIdAndRaceIdAsync(userId, raceId);
        if (fantasyLineup == null)
        {
            throw new NotFoundException("Fantasy lineup not found");
        }
        return CoreGameplayDtoMapper.MapFantasyLineupToGetDto(fantasyLineup);
        
    }
    public async Task<Dtos.Get.FantasyLineupDto> GetCurrentFantasyLineupByUserIdAsync(int userId)
    {
        var fantasyLineup = await fantasyLineupRepository.GetCurrentFantasyLineupByUserIdAsync(userId);
        if (fantasyLineup == null)
        {
            throw new NotFoundException("Fantasy lineup not found");
        }
        return CoreGameplayDtoMapper.MapFantasyLineupToGetDto(fantasyLineup);
    }
    
    public async Task<Dtos.Get.FantasyLineupDto> UpdateFantasyLineupAsync(Dtos.Update.FantasyLineupDto fantasyLineupDto)
    {
        PreValidateFantasyLineupConnections(fantasyLineupDto);
        var trackedFantasyLineup = await fantasyLineupRepository.GetFantasyLineupByIdAsTrackingAsync(fantasyLineupDto.Id);
        if (trackedFantasyLineup == null)
        {
            throw new NotFoundException("Fantasy lineup not found");
        }
        PreValidateFantasyLineupDeadline(trackedFantasyLineup);
        
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var pickableItem = await coreGameplayRepository.GetPickableItemsAsync();
            if (pickableItem == null)
            {
                throw new NotFoundException("Pickables not found");
            }
            ValidatePickableItemsAsync(fantasyLineupDto, pickableItem);
            ValidatePowerUpPickableAsync(fantasyLineupDto);
            
            var maxDrivers = configuration.GetValue<int>("FantasyTeamSettings:MaxDrivers");
            var maxConstructors = configuration.GetValue<int>("FantasyTeamSettings:MaxConstructors");
            
            var updatedFantasyLineup = await fantasyLineupRepository.UpdateFantasyLineupAsync(
                fantasyLineupDto.DriverIds, 
                fantasyLineupDto.ConstructorIds, 
                fantasyLineupDto.PowerupIds, 
                trackedFantasyLineup,
                fantasyLineupDto.CaptainDriverId ?? null,
                maxDrivers,
                maxConstructors);
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return CoreGameplayDtoMapper.MapFantasyLineupToGetDto(updatedFantasyLineup);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating fantasy lineup: {ex.Message}");
            throw;
        }
        
        return CoreGameplayDtoMapper.MapFantasyLineupToGetDto(trackedFantasyLineup);
    }

    private void ValidatePickableItemsAsync(Dtos.Update.FantasyLineupDto fantasyLineupDto, PickableItem pickableItem)
    {
        var pickableDriverIds = pickableItem.Drivers.Select(d => d.Id).ToList();
        var pickableConstructorIds = pickableItem.Constructors.Select(c => c.Id).ToList();
        foreach (var driverId in fantasyLineupDto.DriverIds)
        {
            if (!pickableDriverIds.Contains(driverId))
            {
                throw new Exception($"Driver {driverId} is not pickable or not exist");
            }
        }
        
        foreach (var constructorId in fantasyLineupDto.ConstructorIds)
        {
            if (!pickableConstructorIds.Contains(constructorId))
            {
                throw new Exception($"Constructor {constructorId} is not pickable or not exist");
            }
        }
    }
    
    private void ValidatePowerUpPickableAsync(Dtos.Update.FantasyLineupDto fantasyLineupDto)
    {
        var pickablePowerupIds = coreGameplayRepository.GetAllPowerupsAsync().Result.Select(p => p.Id).ToList();
        foreach (var powerupId in fantasyLineupDto.PowerupIds)
        {
            if (!pickablePowerupIds.Contains(powerupId))
            {
                throw new Exception($"Powerup {powerupId} does not exist");
            }
        }

        var powerupIdsNotAlreadyUsed = coreGameplayRepository.GetAvailablePowerupsByFantasyLineupIdAsync(fantasyLineupDto.Id).Result.Select(p => p.Id).ToList();
        
        // Throw an error including all powerups that are not already used
        var powerupsNotAlreadyUsed = fantasyLineupDto.PowerupIds.Except(powerupIdsNotAlreadyUsed).ToList();
        if (powerupsNotAlreadyUsed.Any())
        {
            throw new Exception($"The following powerups have already been used in the current season: {string.Join(", ", powerupsNotAlreadyUsed)}");
        }
    }
    
    private void PreValidateFantasyLineupConnections(Dtos.Update.FantasyLineupDto fantasyLineupDto)
    {
        if (fantasyLineupDto.CaptainDriverId != null && !fantasyLineupDto.DriverIds.Contains((int)fantasyLineupDto.CaptainDriverId))
        {
            throw new Exception("Captain driver must be one of the selected drivers");
        }
        // Maximum 5 drivers, 2 constructors
        if (fantasyLineupDto.DriverIds.Count > 5)
        {
            throw new Exception("You can select a maximum of 5 drivers");
        }
        if (fantasyLineupDto.ConstructorIds.Count > 2)
        {
            throw new Exception("You can select a maximum of 2 constructors");
        }
        
        // Validate existences of drivers, constructors and powerups in database
        var nonExistingDrivers = coreGameplayRepository.GetNonExistentDriverIdsAsync(fantasyLineupDto.DriverIds);
        if (nonExistingDrivers.Result.Any())
        {
            throw new NotFoundException($"The following driver ids do not exist: {string.Join(", ", nonExistingDrivers.Result)}");
        }
        var nonExistingConstructors = coreGameplayRepository.GetNonExistentConstructorIdsAsync(fantasyLineupDto.ConstructorIds);
        if (nonExistingConstructors.Result.Any())
        {
            throw new NotFoundException($"The following constructor ids do not exist: {string.Join(", ", nonExistingConstructors.Result)}");
        }
        var nonExistingPowerups = coreGameplayRepository.GetNonExistentPowerupIdsAsync(fantasyLineupDto.PowerupIds);
        if (nonExistingPowerups.Result.Any())
        {
            throw new NotFoundException($"The following powerup ids do not exist: {string.Join(", ", nonExistingPowerups.Result)}");
        }
    }

    private void PreValidateFantasyLineupDeadline(FantasyLineup fantasyLineup)
    {
        if (fantasyLineup.Race.DeadlineDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new Exception("The deadline for this fantasy lineup has passed");
        }
    }

     public async Task ResetFantasyLineupsBySeasonYearAsync(int year)
    {
        var season = await staticDataRepository.GetSeasonByYearAsync(year);
        if (season == null)
        {
            throw new NotFoundException($"Season {year} not found");
        }
        
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // Delete all connections to drivers, constructors and powerups from all fantasy lineups from the year inserted
            await fantasyLineupRepository.ResetFantasyLineupsBySeasonAsync(season);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error resetting fantasy lineup for season {year}: {ex.Message}");
            throw;
        }
    }

    public async Task<RaceDto> GetLatestFinishedRaceAsync()
    {
        Race? latestFinishedRace = await coreGameplayRepository.GetLatestFinishedRaceAsync();
        if (latestFinishedRace == null)
        {
            throw new NotFoundException("There is no finished race yet.");
        }
        return CoreGameplayDtoMapper.MapRaceToDto(latestFinishedRace);
    }

    public async Task<RaceResultDto> GetLatestFinishedRaceResultAsync()
    {
        Race ? latestFinishedRaceWithResult = await coreGameplayRepository.GetLatestFinishedRaceResultAsync();
        if (latestFinishedRaceWithResult == null)
        {
            throw new NotFoundException("There is no finished race yet.");
        }
        return CoreGameplayDtoMapper.MapRaceResultToDto(latestFinishedRaceWithResult);
    }

    public async Task<RaceDto> GetLatestRaceAsync()
    {
        var latestRace = await coreGameplayRepository.GetLatestRaceAsync();
        if (latestRace == null)
        {
            throw new NotFoundException("There is no latest race yet.");
        }
        return CoreGameplayDtoMapper.MapRaceToDto(latestRace);
    }
    public async Task<RaceDto> GetCurrentRaceAsync()
    {
        var currentRace = await coreGameplayRepository.GetCurrentRaceAsync();
        if (currentRace == null)
        {
            throw new NotFoundException("There is no current race yet.");
        }
        return CoreGameplayDtoMapper.MapRaceToDto(currentRace);
    }
}