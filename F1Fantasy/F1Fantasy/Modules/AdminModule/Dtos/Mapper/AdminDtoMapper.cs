using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.AdminModule.Dtos.Mapper;

public class AdminDtoMapper
{
    public static Get.ApplicationUserForAdminDto MapUserToApplicationUserForAdminDto(ApplicationUser user, List<ApplicationRole> roles)
    {
        return new Get.ApplicationUserForAdminDto
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

    public static Get.PickableItemDto MapPickableItemToDto(PickableItem pickableItem)
    {
        return new Get.PickableItemDto
        {
            Drivers = pickableItem.Drivers.Select(d => new Get.DriverInPickableItemDto
                {
                    Id = d.Id,
                    GivenName = d.GivenName,
                    FamilyName = d.FamilyName,
                    DateOfBirth = d.DateOfBirth,
                    CountryId = d.CountryId,
                    Code = d.Code,
                    Price = d.Price,
                    ImgUrl = d.ImgUrl
                })
                .ToList(),
            Constructors = pickableItem.Constructors.Select(c => new Get.ConstructorInPickableItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Price = c.Price,
                    ImgUrl = c.ImgUrl,
                    CountryId = c.CountryId
                }).ToList(),
        };
    }
}
