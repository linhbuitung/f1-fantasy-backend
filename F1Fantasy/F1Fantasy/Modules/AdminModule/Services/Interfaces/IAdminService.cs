using F1Fantasy.Modules.AdminModule.Dtos;

namespace F1Fantasy.Modules.AdminModule.Services.Interfaces;

public interface IAdminService
{
    Task<SeasonDto>  StartSeasonAsync(int year);
    
    Task<SeasonDto> GetActiveSeasonAsync();
    
    Task DeactivateActiveSeasonAsync();
    
    Task<Dtos.Get.ApplicationUserForAdminDto> UpdateUserRoleAsync(int userId, List<string> roleNames);
    
    Task<Dtos.Get.PickableItemDto> GetPickableItemAsync();
    
    Task<Dtos.Get.PickableItemDto> UpdatePickableItemAsync(Dtos.Update.PickableItemDto dto);
    
    Task ResetPickableItemsAsync();
    Task<Dtos.Get.PickableItemDto> UpdatePickableItemFromAllDriversInASeasonYearAsync(int seasonYear);

    Task<Dtos.Get.DriverDto> UpdateDriverInfoAsync(Dtos.Update.DriverDto dto);
    
    Task<Dtos.Get.ConstructorDto> UpdateConstructorInfoAsync(Dtos.Update.ConstructorDto dto);
    
    Task<Dtos.Get.CircuitDto> UpdateCircuitInfosync(Dtos.Update.CircuitDto dto);
    
    Task<Dtos.Get.PowerupDto> UpdatePowerupInfoAsync(Dtos.Update.PowerupDto dto);
}