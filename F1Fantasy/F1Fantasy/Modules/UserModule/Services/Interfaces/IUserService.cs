using F1Fantasy.Core.Common;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.UserModule.Dtos;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.UserModule.Services.Interfaces;

public interface IUserService
{
    Task<Dtos.Get.ApplicationUserDto> UpdateUserAsync( Dtos.Update.ApplicationUserDto userUpdateDto);

    Task<Dtos.Get.ApplicationUserDto> GetUserByIdAsync(int id);
    
    Task<List<Dtos.Get.ApplicationUserDto>> FindUserByDisplayNameAsync(string name);
    
}