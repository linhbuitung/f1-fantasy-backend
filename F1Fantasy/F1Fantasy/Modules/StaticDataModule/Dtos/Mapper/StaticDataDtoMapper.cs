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
            return new Driver(driverDto.GivenName, driverDto.FamilyName, driverDto.DateOfBirth, driverDto.Code, driverDto.ImgUrl);
        }

        public static ConstructorDto MapConstructorToDto(Constructor constructor)
        {
            return new ConstructorDto(constructor.Id, constructor.Name, constructor.CountryId, constructor.Code, constructor.ImgUrl);
        }

        public static Constructor MapDtoToConstructor(ConstructorDto constructorDto)
        {
            return new Constructor(constructorDto.Name, constructorDto.Code, constructorDto.ImgUrl);
        }

        public static CircuitDto MapCircuitToDto(Circuit circuit)
        {
            return new CircuitDto(circuit.Id, circuit.CircuitName, circuit.Code, circuit.Latitude, circuit.Longtitude, circuit.Locality, circuit.CountryId, circuit.ImgUrl);
        }

        public static Circuit MapDtoToCircuit(CircuitDto circuitDto)
        {
            return new Circuit(circuitDto.CircuitName, circuitDto.Code, circuitDto.Latitude, circuitDto.Longtitude, circuitDto.Locality, circuitDto.CountryId, circuitDto.ImgUrl);
        }

        public static Country MapDtoToNationality(NationalityDto nationalityDto)
        {
            return new Country
            {
                Id = nationalityDto.NationalityId,
                Nationalities = nationalityDto.Names
            };
        }

        public static NationalityDto MapNationalityToDto(Country nationality)
        {
            return new NationalityDto
            {
                NationalityId = nationality.Id,
                Names = nationality.Nationalities
            };
        }
    }
}