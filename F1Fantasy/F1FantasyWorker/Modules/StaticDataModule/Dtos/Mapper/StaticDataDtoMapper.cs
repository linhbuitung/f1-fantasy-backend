using F1FantasyWorker.Core.Common;
using Newtonsoft.Json.Linq;

namespace F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper
{
    public class StaticDataDtoMapper
    {
        public static DriverDto MapDriverToDto(Driver driver)
        {
            return new DriverDto(driver.Id, driver.GivenName, driver.FamilyName, driver.DateOfBirth, driver.Nationality, driver.Code, driver.ImgUrl);
        }

        public static Driver MapDtoToDriver(DriverDto driverDto)
        {
            //return new Driver(driverDto.GivenName, driverDto.FamilyName, driverDto.DateOfBirth, driverDto.Nationality, driverDto.Code, driverDto.ImgUrl);
            return new Driver
            {
                GivenName = driverDto.GivenName,
                FamilyName = driverDto.FamilyName,
                DateOfBirth = driverDto.DateOfBirth,
                Nationality = driverDto.Nationality,
                Code = driverDto.Code,
                ImgUrl = driverDto.ImgUrl
            };
        }

        public static ConstructorDto MapConstructorToDto(Constructor constructor)
        {
            return new ConstructorDto(constructor.Id, constructor.Name, constructor.Nationality, constructor.Code, constructor.ImgUrl);
        }

        public static Constructor MapDtoToConstructor(ConstructorDto constructorDto)
        {
            // return new Constructor(constructorDto.Name, constructorDto.Nationality, constructorDto.Code, constructorDto.ImgUrl);
            return new Constructor
            {
                Name = constructorDto.Name,
                Nationality = constructorDto.Nationality,
                Code = constructorDto.Code,
                ImgUrl = constructorDto.ImgUrl
            };
        }

        public static CircuitDto MapCircuitToDto(Circuit circuit)
        {
            return new CircuitDto(circuit.Id, circuit.CircuitName, circuit.Code, circuit.Latitude, circuit.Longtitude, circuit.Locality, circuit.Country, circuit.ImgUrl);
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
                Country = circuitDto.Country,
                ImgUrl = circuitDto.ImgUrl
            };
        }
    }
}