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

    public static ApplicationUser MapUpdateDtoToUser(ApplicationUserUpdateDto dto)
    {
        return new ApplicationUser
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName,
            DateOfBirth = dto.DateOfBirth,
            AcceptNotification = dto.AcceptNotification,
            ConstructorId = dto.ConstructorId,
            DriverId = dto.DriverId,
            CountryId = dto.CountryId
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