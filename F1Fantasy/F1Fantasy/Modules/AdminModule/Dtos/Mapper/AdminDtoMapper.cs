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

    public static Driver MapUpdateDtoToDriver(Update.DriverDto driver)
    {
        return new Driver
        {
            Id = driver.Id,
            Price = driver.Price ?? 0,
            ImgUrl = driver.ImgUrl
        };
    }
    public static Get.DriverDto MapDriverToGetDto(Driver driver)
    {
        return new Get.DriverDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            DateOfBirth = driver.DateOfBirth,
            CountryId = driver.CountryId,
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Constructor MapUpdateDtoToConstructor(Update.ConstructorDto constructor)
    {
        return new Constructor
        {
            Id = constructor.Id,
            Price = constructor.Price ?? 0,
            ImgUrl = constructor.ImgUrl
        };
    }
    
    public static Get.ConstructorDto MapConstructorToGetDto(Constructor constructor)
    {
        return new Get.ConstructorDto
        {
            Id = constructor.Id,
            Name = constructor.Name,
            Code = constructor.Code,
            CountryId = constructor.CountryId,
            Price = constructor.Price,
            ImgUrl = constructor.ImgUrl
        };
    }
    
    public static Circuit MapUpdateDtoToCircuit(Update.CircuitDto circuit)
    {
        return new Circuit
        {
            Id = circuit.Id,
            ImgUrl = circuit.ImgUrl
        };
    }
    
    public static Get.CircuitDto MapCircuitToGetDto(Circuit circuit)
    {
        return new Get.CircuitDto
        {
            Id = circuit.Id,
            CircuitName = circuit.CircuitName,
            Code = circuit.Code,
            Latitude = circuit.Latitude,
            Longitude = circuit.Longitude,
            Locality = circuit.Locality,
            CountryId = circuit.CountryId,
            ImgUrl = circuit.ImgUrl
        };
    }

    public static Powerup MapUpdateDtoToPowerup(Update.PowerupDto powerup)
    {
        return new Powerup
        {
            Id = powerup.Id,
            ImgUrl = powerup.ImgUrl
        };
    }
}
