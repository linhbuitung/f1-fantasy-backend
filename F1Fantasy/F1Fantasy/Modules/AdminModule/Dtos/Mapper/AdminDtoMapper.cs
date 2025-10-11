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

    public static Driver MapUpdateDtoToDriver(Update.DriverDto driver, string imgUrl)
    {
        return new Driver
        {
            Id = driver.Id,
            ImgUrl = imgUrl
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
    
    public static Constructor MapUpdateDtoToConstructor(Update.ConstructorDto constructor, string imgUrl)
    {
        return new Constructor
        {
            Id = constructor.Id,
            ImgUrl = imgUrl
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
    
    public static Circuit MapUpdateDtoToCircuit(Update.CircuitDto circuit, string imgUrl)
    {
        return new Circuit
        {
            Id = circuit.Id,
            ImgUrl = imgUrl
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

    public static Powerup MapUpdateDtoToPowerup(Update.PowerupDto powerup, string imgUrl)
    {
        return new Powerup
        {
            Id = powerup.Id,
            ImgUrl = imgUrl
        };
    }
    
    public static Get.PowerupDto MapPowerupToGetDto(Powerup powerup)
    {
        return new Get.PowerupDto
        {
            Id = powerup.Id,
            Type = powerup.Type,
            Description = powerup.Description,
            ImgUrl = powerup.ImgUrl
        };
    }
}
