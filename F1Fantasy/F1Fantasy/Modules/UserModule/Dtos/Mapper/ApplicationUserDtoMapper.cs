using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.UserModule.Dtos.Mapper;

public class ApplicationUserDtoMapper
{
    public static Get.ApplicationUserDto MapUserToGetDto(ApplicationUser user)
    {
        return new Get.ApplicationUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            DateOfBirth = user.DateOfBirth,
            ConsecutiveActiveDays = user.ConsecutiveActiveDays,
            LastActiveAt = user.LastActiveAt,
            AskAiCredits = user.AskAiCredits,
            AcceptNotification = user.AcceptNotification,
            ConstructorId = user.ConstructorId ?? null,
            ConstructorName = user.Constructor?.Name ?? null,
            DriverId = user.DriverId,
            DriverName = user.Driver == null ? null : string.Concat(user.Driver.GivenName, " ", user.Driver.FamilyName),            CountryId = user.CountryId,
            CountryName = user.Country?.ShortName  ?? null
        };
    }

    public static ApplicationUser MapUpdateDtoToUser(Update.ApplicationUserDto dto, ApplicationUser existingUser)
    {
        return new ApplicationUser
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName ?? existingUser.DisplayName,
            DateOfBirth = dto.DateOfBirth ?? existingUser.DateOfBirth,
            AcceptNotification = dto.AcceptNotification ?? existingUser.AcceptNotification,
            ConstructorId = dto.ConstructorId ?? existingUser.ConstructorId,
            DriverId = dto.DriverId ?? existingUser.DriverId,
            CountryId = dto.CountryId ?? existingUser.CountryId
        };
    }

    public static List<Get.ApplicationUserDto> MapListUsersToDto(List<ApplicationUser> users)
    {
        List<Get.ApplicationUserDto> dtos = new List<Get.ApplicationUserDto>();

        foreach (var user in users)
        {
            dtos.Add(MapUserToGetDto(user));
        }
        
        return dtos;
    }
}