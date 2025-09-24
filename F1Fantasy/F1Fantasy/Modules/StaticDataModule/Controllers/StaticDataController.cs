using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StaticDataModule.Controllers;

[ApiController]
[Route("api/static-data")]
public class StaticDataController(
    IDriverService driverService, 
    IConstructorService constructorService, 
    ICountryService countryService) : ControllerBase
{
    [HttpGet("drivers")]
    public async Task<IActionResult> GetAllDrivers()
    {
        var driverDtos = await driverService.GetAllDriversAsync();

        return Ok(driverDtos);
    }
    
    [HttpGet("constructors")]
    public async Task<IActionResult> GetAllConstructors()
    {
        var constructorDtos = await constructorService.GetAllConstructorsAsync();

        return Ok(constructorDtos);
    }

    [HttpGet("countries")]
    public async Task<IActionResult> GetAllCountries()
    {
        var countryDtos = await countryService.GetAllCountriesAsync();
        return Ok(countryDtos);
    }
}