using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AdminModule.Dtos;
using F1Fantasy.Modules.AdminModule.Dtos.Get;
using F1Fantasy.Modules.AdminModule.Dtos.Mapper;
using F1Fantasy.Modules.AdminModule.Extensions.Interfaces;
using F1Fantasy.Modules.AdminModule.Repositories.Interfaces;
using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace F1Fantasy.Modules.AdminModule.Services.Implementations;

public class AdminService(IAdminRepository adminRepository, IStaticDataRepository staticDataRepository, ICoreGameplayService coreGameplayService, WooF1Context context, IConfiguration configuration, ICloudStorage cloudStorage) : IAdminService
{
    public async Task<SeasonDto> StartSeasonAsync(int year)
    {
        Season? currentActiveSeason = await adminRepository.GetActiveSeasonAsync();
        if (currentActiveSeason != null)
        {
            throw new InvalidOperationException("There is already an active season. Please deactivate it before starting a new one.");
        }
        
        Season activeSeason = await adminRepository.UpdateSeasonStatusAsync(year, isActive: true);
        await coreGameplayService.ResetFantasyLineupsBySeasonYearAsync(activeSeason.Year);
        
        SeasonDto returnDto = AdminDtoMapper.MapSeasonToDto(activeSeason);
        return returnDto;
    }

    public async Task<SeasonDto> GetActiveSeasonAsync()
    {
        Season? currentlyActiveSeason = await adminRepository.GetActiveSeasonAsync();
        if (currentlyActiveSeason == null)
        {
            throw new NotFoundException("There is no active season.");
        }
        return AdminDtoMapper.MapSeasonToDto(currentlyActiveSeason);
    }
    
    public async Task DeactivateActiveSeasonAsync()
    {
        Season? currentlyActiveSeason = await adminRepository.GetActiveSeasonAsync();
        if (currentlyActiveSeason == null)
        {
            throw new NotFoundException("There is no active season.");
        }
        
        // Deactivate all seasons
        await adminRepository.UpdateSeasonStatusAsync(currentlyActiveSeason.Year, isActive: false);
    }
    public async Task<Dtos.Get.ApplicationUserForAdminDto> UpdateUserRoleAsync(int userId, List<string> roleNames)
    {
        // Always ensure Player role is present
        if (!roleNames.Contains(AppRoles.Player))
            roleNames.Add(AppRoles.Player);
        
        // verify all roles exist
        List<string> notFoundRoles = await VerifyRolesExistAsync(roleNames);
        if (notFoundRoles.Count > 0)
        {
            throw new ArgumentException($"Roles not found in the database: {string.Join(", ", notFoundRoles)}");
        }
        
        ApplicationUser updatedUser = await adminRepository.UpdateUserRoleAsync(userId, roleNames);

        var returnDto = AdminDtoMapper.MapUserToApplicationUserForAdminDto(updatedUser, updatedUser.UserRoles.Select(ur => ur.Role).ToList());
        
        return returnDto;
    }
    
    // This return list of roles that are not found in the database
    // If all roles exist, it returns an empty list
    private async Task<List<string>> VerifyRolesExistAsync(List<string> roleNames)
    {
        List<ApplicationRole> roles = await adminRepository.GetAllRolesAsync();
        List<string> notFoundRoles = new List<string>();
        foreach (var roleName in roleNames)
        {
            if (roles.All(r => r.Name != roleName))
            {
                notFoundRoles.Add(roleName);
            }
        }
        return notFoundRoles;
    }

    public async Task<Dtos.Get.PickableItemDto> GetPickableItemAsync()
    {
        var pickableItem = await adminRepository.GetPickableItemAsync();
        if (pickableItem == null)
        {
            throw new NotFoundException("Pickable item not found.");
        }
        return AdminDtoMapper.MapPickableItemToDto(pickableItem);
    }

    public async Task<Dtos.Get.PickableItemDto> UpdatePickableItemAsync(Dtos.Update.PickableItemDto dto)
    {
        var defaultPrice = configuration.GetValue<int?>("AdminSettings:DefaultPrice") ?? 15;
        // check if is development environment
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" )
        {
            await PreValidationForActionsAfterLatestRace("Pickable items");
        }
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var pickableItem = await adminRepository.GetPickableItemAsync();
            if (pickableItem == null)
            {
                throw new NotFoundException("Pickable item not found.");
            }
            
            // Delete all driver connections in the pickable item that are not in the dto
            var driversToRemove = pickableItem.Drivers.Where(d => !dto.DriverIds.Contains(d.Id)).ToList();
            foreach (var driver in driversToRemove)
            {
                pickableItem.Drivers.Remove(driver);
            }
            
            // Add new driver connections from the dto that are not already in the pickable item
            var existingDriverIds = pickableItem.Drivers.Select(d => d.Id).ToHashSet();
            var driversToAdd = dto.DriverIds.Where(id => !existingDriverIds.Contains(id)).ToList();
            foreach (var driverId in driversToAdd)
            {
                var driver = await staticDataRepository.GetDriverByIdAsTrackingAsync(driverId);
                if (driver == null)
                {
                    throw new NotFoundException($"Driver with ID {driverId} not found.");
                }

                driver.Price = defaultPrice;
                pickableItem.Drivers.Add(driver);
            }
            
            // Delete all constructor connections in the pickable item that are not in the dto
            var constructorsToRemove = pickableItem.Constructors.Where(c => !dto.ConstructorIds.Contains(c.Id)).ToList();
            foreach (var constructor in constructorsToRemove)
            {
                pickableItem.Constructors.Remove(constructor);
            }
            
            // Add new constructor connections from the dto that are not already in the pickable item
            var existingConstructorIds = pickableItem.Constructors.Select(c => c.Id).ToHashSet();
            var constructorsToAdd = dto.ConstructorIds.Where(id => !existingConstructorIds.Contains(id)).ToList();
            foreach (var constructorId in constructorsToAdd)
            {
                var constructor = await staticDataRepository.GetConstructorByIdAsTrackingAsync(constructorId);
                if (constructor == null)
                {
                    throw new NotFoundException($"Constructor with ID {constructorId} not found.");
                }   
                
                constructor.Price = defaultPrice;
                pickableItem.Constructors.Add(constructor);
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return AdminDtoMapper.MapPickableItemToDto(pickableItem);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating pickable item: {ex.Message}");
            throw;
        }
    }

    public async Task<Dtos.Get.PickableItemDto> UpdatePickableItemFromAllDriversInASeasonYearAsync(int seasonYear)
    {
        var season = await staticDataRepository.GetSeasonByYearAsync(seasonYear);
        if (season == null)
        {
            throw new NotFoundException($"Season with year {seasonYear} not found.");
        }
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" )
        {
            await PreValidationForActionsAfterLatestRace("Pickable items");
        }
        
        var allDriverIdsInSeason = (await staticDataRepository.GetAllDriversBySeasonIdAsync(season.Id)).Select(d => d.Id).ToList();
        var allConstructorIdsInSeason = (await staticDataRepository.GetAllConstructorsBySeasonIdAsync(season.Id)).Select(c => c.Id).ToList();
        
        var pickableItemDto = new Dtos.Update.PickableItemDto
        {
            DriverIds = allDriverIdsInSeason,
            ConstructorIds = allConstructorIdsInSeason
        };
        
        return await UpdatePickableItemAsync(pickableItemDto);
    }

    private async Task PreValidationForActionsAfterLatestRace(string objectNameForMessage)
    {
        // Ensure that admin can only doc action if current date is between a race date and 1 day after it
        var activeSeason = await adminRepository.GetActiveSeasonAsync();
        if (activeSeason == null)
        {
            throw new InvalidOperationException("There is no active season.");
        }
        var latestRace = await coreGameplayService.GetLatestFinishedRaceAsync();
        
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        if (!(latestRace.RaceDate < currentDate && currentDate < latestRace.RaceDate.AddDays(2)))
        {
            throw new InvalidOperationException($"{objectNameForMessage} can only be modified in {latestRace.RaceDate.AddDays(1)}, after latest race with id {latestRace.Id}." );
        }
    }

    public async Task<Dtos.Get.DriverDto> UpdateDriverInfoAsync(Dtos.Update.DriverDto dto)
    {
        
        var existingDriver = await staticDataRepository.GetDriverByIdAsync(dto.Id);
        if (existingDriver == null)
        {
            throw new NotFoundException($"Driver with ID {dto.Id} not found.");
        }
        
        var imgUrl = await cloudStorage.UploadFileAsync(dto.Img, "imgs/drivers/" + existingDriver.Code);
        var updatedDriver = AdminDtoMapper.MapUpdateDtoToDriver(dto, imgUrl);
        var resultDriver = await adminRepository.UpdateDriverInfoAsync(updatedDriver);
        
        return AdminDtoMapper.MapDriverToGetDto(resultDriver);
    }

    public async Task<Dtos.Get.ConstructorDto> UpdateConstructorInfoAsync(Dtos.Update.ConstructorDto dto)
    {
        var existingConstructor = await staticDataRepository.GetConstructorByIdAsync(dto.Id);
        if (existingConstructor == null)
        {
            throw new NotFoundException($"Constructor with ID {dto.Id} not found.");
        }
        
        var imgUrl = await cloudStorage.UploadFileAsync(dto.Img, "imgs/constructors/" + existingConstructor.Code);
        var updatedConstructor = AdminDtoMapper.MapUpdateDtoToConstructor(dto, imgUrl);
        var resultConstructor = await adminRepository.UpdateConstructorInfoAsync(updatedConstructor);
        
        return AdminDtoMapper.MapConstructorToGetDto(resultConstructor);
    }

    public async Task<Dtos.Get.CircuitDto> UpdateCircuitInfosync(Dtos.Update.CircuitDto dto)
    {
        var existingCircuit = await staticDataRepository.GetCircuitByIdAsync(dto.Id);
        if (existingCircuit == null)
        {
            throw new NotFoundException($"Circuit with ID {dto.Id} not found.");
        }
        var imgUrl = await cloudStorage.UploadFileAsync(dto.Img, "imgs/circuits/" + existingCircuit.Code);

        var updatedCircuit = AdminDtoMapper.MapUpdateDtoToCircuit(dto, imgUrl);
        var resultCircuit = await adminRepository.UpdateCircuitInfoAsync(updatedCircuit);
        
        return AdminDtoMapper.MapCircuitToGetDto(resultCircuit);
    }

    public async Task<Dtos.Get.PowerupDto> UpdatePowerupInfoAsync(Dtos.Update.PowerupDto dto)
    {
        var existingPowerup = await staticDataRepository.GetPowerupByIdAsync(dto.Id);
        if (existingPowerup == null)
        {
            throw new NotFoundException($"Powerup with ID {dto.Id} not found.");
        }
        
        var imgUrl = await cloudStorage.UploadFileAsync(dto.Img, $"imgs/powerups/{existingPowerup.Id}");

        var updatedPowerup = AdminDtoMapper.MapUpdateDtoToPowerup(dto,imgUrl);
        var resultPowerup = await adminRepository.UpdatePowerupInfoAsync(updatedPowerup);
        
        return AdminDtoMapper.MapPowerupToGetDto(resultPowerup);
    }
    
}