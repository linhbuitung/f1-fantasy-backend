using F1Fantasy.Core.Common;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.UserModule.Dtos;

namespace F1Fantasy.Modules.UserModule.Services.Interfaces;

public interface IUserService
{
    Task<ApplicationUserGetDto> UpdateUserAsync(ApplicationUserUpdateDto userUpdateDto);

    Task<ApplicationUserGetDto> GetUserByIdAsync(int id);
    
    Task<List<ApplicationUserGetDto>> FindUserByDisplayNameAsync(string name);
}