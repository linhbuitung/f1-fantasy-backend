using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.AdminModule.Dtos.Mapper;

public class AdminDtoMapper
{
    public static ApplicationUserForAdminGetDto MapUserToApplicationUserForAdminDto(ApplicationUser user, List<ApplicationRole> roles)
    {
        return new ApplicationUserForAdminGetDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Roles = roles.Select(r => r.Name).ToList(),
        };
    }
    
    public static SeasonDto MapSeasonToDto(Season season)
    {
        return new SeasonDto
        {
            Id = season.Id,
            Year = season.Year,
            IsActive = season.IsActive,
        };
    }
    
    public static Season MapDtoToSeason(SeasonDto seasonDto)
    {
        return new Season
        {
            Year = seasonDto.Year,
            IsActive = seasonDto.IsActive,
        };
    }

}