using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.UserModule.Dtos.Mapper;

public class ApplicationUserDtoMapper
{
    public static ApplicationUserGetDto MapUserToGetDto(ApplicationUser user)
    {
        return new ApplicationUserGetDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            DateOfBirth = user.DateOfBirth,
            LoginStreak = user.LoginStreak,
            LastLogin = user.LastLogin,
            AcceptNotification = user.AcceptNotification,
            ConstructorId = user.ConstructorId ?? null,
            ConstructorName = user.Constructor?.Name ?? null,
            DriverId = user.DriverId,
            DriverName = user.Driver == null ? null : string.Concat(user.Driver.GivenName, " ", user.Driver.FamilyName),            CountryId = user.CountryId,
            CountryName = user.Country?.Nationalities[0]  ?? null
        };
    }

    public static ApplicationUser MapUpdateDtoToUser(ApplicationUserUpdateDto dto, ApplicationUser existingUser)
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

    public static List<ApplicationUserGetDto> MapListUsersToDto(List<ApplicationUser> users)
    {
        List<ApplicationUserGetDto> dtos = new List<ApplicationUserGetDto>();

        foreach (var user in users)
        {
            dtos.Add(MapUserToGetDto(user));
        }
        
        return dtos;
    }
}