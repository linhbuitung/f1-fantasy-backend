using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.StaticDataModule.Controllers;

[ApiController]
[Route("static-data")]
public class StaticDataController(
    IDriverService driverService, 
    IConstructorService constructorService, 
    ICountryService countryService,
    IPowerupService powerupService,
    ICircuitService circuitService) : ControllerBase
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
    
    [HttpGet("powerups")]
    public async Task<IActionResult> GetAllPowerups()
    {
        var powerupDtos = await powerupService.GetAllPowerupsAsync();
        return Ok(powerupDtos);
    }
    
    [HttpGet("/driver/search")]
    public async Task<IActionResult> SearchDrivers(
        [FromQuery] string query,
        [FromQuery] int pageNum = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required.");

        var result = await driverService.SearchDriversAsync(query, pageNum, pageSize);
        return Ok(result);
    }
    
    [HttpGet("/constructor/search")]
    public async Task<IActionResult> SearchConstructors(
        [FromQuery] string query,
        [FromQuery] int pageNum = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required.");

        var result = await constructorService.SearchConstructorsAsync(query, pageNum, pageSize);
        return Ok(result);
    }
    
    [HttpGet("/circuit/search")]
    public async Task<IActionResult> SearchCircuits(
        [FromQuery] string query,
        [FromQuery] int pageNum = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required.");

        var result = await circuitService.SearchCircuitsAsync(query, pageNum, pageSize);
        return Ok(result);
    }
}