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
    
    Task<Dtos.Get.PickableItemDto> UpdatePickableItemFromAllDriversInASeasonYearAsync(int seasonYear);

}