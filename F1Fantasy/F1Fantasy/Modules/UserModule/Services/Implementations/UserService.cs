using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.UserModule.Dtos;
using F1Fantasy.Modules.UserModule.Dtos.Mapper;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;
using F1Fantasy.Modules.UserModule.Services.Interfaces;

namespace F1Fantasy.Modules.UserModule.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly WooF1Context _context;

    public UserService( IUserRepository userRepository, WooF1Context context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<Dtos.Get.ApplicationUserDto> UpdateUserAsync(Dtos.Update.ApplicationUserDto userUpdateDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            ApplicationUser? existingUser = await _userRepository.GetUserByIdAsync(userUpdateDto.Id);
            if (existingUser is null)
            {
                throw new NotFoundException($"User with id {userUpdateDto.Id} not found");
            }
                
            ApplicationUser user = ApplicationUserDtoMapper.MapUpdateDtoToUser(userUpdateDto, existingUser);

            ApplicationUser newUser = await _userRepository.UpdateUserAsync(user);

            // Additional operations that need atomicity (example: logging the event)
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return ApplicationUserDtoMapper.MapUserToGetDto(newUser);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating user: {ex.Message}");

            throw;
        }
    }

    public async Task<Dtos.Get.ApplicationUserDto> GetUserByIdAsync(int id)
    {
        ApplicationUser? user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with id {id} not found");
        }
        return ApplicationUserDtoMapper.MapUserToGetDto(user);
    }

    public async Task<List<Dtos.Get.ApplicationUserDto>> FindUserByDisplayNameAsync(string name)
    {
        List<ApplicationUser> users = await _userRepository.FindUserByDisplayNameAsync(name);
        if (users == null)
        {
            return null;
        }
        return ApplicationUserDtoMapper.MapListUsersToDto(users);
    }
}