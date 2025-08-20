using F1Fantasy.Core.Common;
using Newtonsoft.Json.Linq;

namespace F1Fantasy.Modules.StaticDataModule.Dtos.Mapper
{
    public class StaticDataDtoMapper
    {
        public static DriverDto MapDriverToDto(Driver driver)
        {
            return new DriverDto(driver.Id, driver.GivenName, driver.FamilyName, driver.DateOfBirth, driver.CountryId, driver.Code, driver.ImgUrl);
        }

        public static Driver MapDtoToDriver(DriverDto driverDto)
        {
            //return new Driver(driverDto.GivenName, driverDto.FamilyName, driverDto.DateOfBirth, driverDto.Nationality, driverDto.Code, driverDto.ImgUrl);
            return new Driver
            {
                GivenName = driverDto.GivenName,
                FamilyName = driverDto.FamilyName,
                DateOfBirth = driverDto.DateOfBirth,
                CountryId = driverDto.CountryId,
                Code = driverDto.Code,
                ImgUrl = driverDto.ImgUrl
            };
        }

        public static ConstructorDto MapConstructorToDto(Constructor constructor)
        {
            return new ConstructorDto(constructor.Id, constructor.Name, constructor.CountryId, constructor.Code, constructor.ImgUrl);
        }

        public static Constructor MapDtoToConstructor(ConstructorDto constructorDto)
        {
            // return new Constructor(constructorDto.Name, constructorDto.Nationality, constructorDto.Code, constructorDto.ImgUrl);
            return new Constructor
            {
                Name = constructorDto.Name,
                CountryId = constructorDto.CountryId,
                Code = constructorDto.Code,
                ImgUrl = constructorDto.ImgUrl
            };
        }

        public static CircuitDto MapCircuitToDto(Circuit circuit)
        {
            return new CircuitDto(circuit.Id, circuit.CircuitName, circuit.Code, circuit.Latitude, circuit.Longtitude, circuit.Locality, circuit.CountryId, circuit.ImgUrl);
        }

        public static Circuit MapDtoToCircuit(CircuitDto circuitDto)
        {
            return new Circuit
            {
                CircuitName = circuitDto.CircuitName,
                Code = circuitDto.Code,
                Latitude = circuitDto.Latitude,
                Longtitude = circuitDto.Longtitude,
                Locality = circuitDto.Locality,
                CountryId = circuitDto.CountryId,
                ImgUrl = circuitDto.ImgUrl
            };
        }

        public static CountryDto MapCountryToDto(Country country)
        {
            return new CountryDto(country.Id, country.Nationalities, country.ShortName);
        }

        public static Country MapDtoToCountry(CountryDto countryDto)
        {
            return new Country
            {
                Id = countryDto.CountryId,
                Nationalities = countryDto.Nationalities,
                ShortName = countryDto.ShortName
            };
        }


        public static RaceDto MapRaceToDto(Race race)
        {
            return new RaceDto(race.Id, race.RaceDate, race.DeadlineDate, race.Calculated, race.SeasonId, race.CircuitId, null);
        }

        public static Race MapDtoToRace(RaceDto raceDto)
        {
            return new Race()
            {
                RaceDate = raceDto.RaceDate,
                Calculated = raceDto.Calculated,
                SeasonId = raceDto.SeasonId ?? 0, // Default to 0 if SeasonId is null
                CircuitId = raceDto.CircuitId ?? 0, // Default to 0 if CircuitId is null
                DeadlineDate = raceDto.DeadlineDate
            };
        }

        public static PowerupDto MapPowerupToDto(Powerup powerup)
        {
            return new PowerupDto(powerup.Id, powerup.Type, powerup.Description, powerup.ImgUrl);
        }
        
        public static Powerup MapDtoToPowerup(PowerupDto powerupDto)
        {
            return new Powerup
            {
                Id = powerupDto.Id ?? 0, // Default to 0 if Id is null
                Type = powerupDto.Type,
                Description = powerupDto.Description,
                ImgUrl = powerupDto.ImgUrl
            };
        }
        
        public static SeasonDto MapSeasonToDto(Season season)
        {
            return new SeasonDto(season.Id, season.Year, season.IsActive);
        }
        
        public static Season MapDtoToSeason(SeasonDto seasonDto)
        {
            return new Season
            {
                Year = seasonDto.Year,
                IsActive = seasonDto.IsActive
            };
        }
    }
}