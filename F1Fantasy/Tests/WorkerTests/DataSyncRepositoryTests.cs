using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Implementations;
using F1FantasyWorker.Core.Common;

namespace Tests.WorkerTests;

public class DataSyncRepositoryTests
{
    private WooF1Context CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WooF1Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WooF1Context(options);
    }

    [Fact]
    public async Task AddDriverAsync_AddsDriverToDb()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);

        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = ["Thai"] };
        await repo.AddCountryAsync(country);
        var driver = new Driver { Code = "TST", GivenName = "Test", FamilyName = "Driver", CountryId = country.Id };
        var added = await repo.AddDriverAsync(driver);

        Assert.NotNull(added);
        Assert.True(added.Id > 0);

        var db = await ctx.Drivers.FindAsync(added.Id);
        Assert.NotNull(db);
        Assert.Equal("TST", db.Code);
    }

    [Fact]
    public async Task GetDriverByCodeAsync_ReturnsDriverWhenExists()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);
        
        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = ["Thai"] };
        await repo.AddCountryAsync(country);
        var driverToAdd = new Driver { Code = "TST", GivenName = "Test", FamilyName = "Driver", CountryId = country.Id };
        var added = await repo.AddDriverAsync(driverToAdd);
        
        await ctx.SaveChangesAsync();

        var driver = await repo.GetDriverByCodeAsync("TST");

        Assert.NotNull(driver);
        Assert.Equal("TST", driver.Code);
    }

    [Fact]
    public async Task GetAllDriverCodesAsync_ReturnsAllCodes()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);
        
        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = ["Thai"] };
        await repo.AddCountryAsync(country);

        ctx.Drivers.AddRange(
            new Driver {  Code = "TST1", GivenName = "Test1", FamilyName = "Driver1", CountryId = country.Id  },
            new Driver {  Code = "TST2", GivenName = "Test2", FamilyName = "Driver2", CountryId = country.Id  });
        await ctx.SaveChangesAsync();
        
        var codes = await repo.GetAllDriverCodesAsync();

        Assert.Contains("TST1", codes);
        Assert.Contains("TST1", codes);
        Assert.Equal(2, codes.Count);
    }
    
    
    [Fact]
    public async Task AddListDriversAsync_AddsMultipleDrivers()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);

        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = new List<string> { "Thai" } };
        await repo.AddCountryAsync(country);

        var drivers = new List<Driver>
        {
            new Driver { Code = "D1", GivenName = "A", FamilyName = "B", CountryId = country.Id },
            new Driver { Code = "D2", GivenName = "C", FamilyName = "D", CountryId = country.Id }
        };

        var added = await repo.AddListDriversAsync(drivers);

        Assert.Equal(2, added.Count);
        var codes = await repo.GetAllDriverCodesAsync();
        Assert.Contains("D1", codes);
        Assert.Contains("D2", codes);
    }

    [Fact]
    public async Task AddListConstructorsAsync_AddsMultipleConstructors()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);

        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = new List<string> { "Thai" } };
        await repo.AddCountryAsync(country);
        
        var ctors = new List<Constructor>
        {
            new Constructor { Code = "C1", Name = "One", CountryId = country.Id  },
            new Constructor { Code = "C2", Name = "Two", CountryId = country.Id  }
        };

        var added = await repo.AddListConstructorsAsync(ctors);

        Assert.Equal(2, added.Count);
        var ctorCodes = await repo.GetAllConstructorCodesAsync();
        Assert.Contains("C1", ctorCodes);
        Assert.Contains("C2", ctorCodes);
    }

    [Fact]
    public async Task AddListCircuitsAsync_GetAllCircuitCodesAsync_Works()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);

        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = new List<string> { "Thai" } };
        await repo.AddCountryAsync(country);
        
        var circuits = new List<Circuit>
        {
            new Circuit { Code = "CT1", CircuitName = "Circuit1", CountryId = country.Id, Locality = "Loc1", Latitude = 0, Longitude = 0 },
            new Circuit { Code = "CT2", CircuitName = "Circuit2", CountryId = country.Id, Locality = "Loc2", Latitude = 0, Longitude = 0 },
        };

        await repo.AddListCircuitsAsync(circuits);

        var codes = await repo.GetAllCircuitCodesAsync();
        Assert.Contains("CT1", codes);
        Assert.Contains("CT2", codes);
    }

    [Fact]
    public async Task AddListRaceEntriesAsync_AddsEntries_And_GetAllRaceDatesAsync()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);

        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = new List<string> { "Thai" } };
        await repo.AddCountryAsync(country);
        
        var season = new Season { Year = 2024 };
        await repo.AddSeasonAsync(season);

        var circuit = new Circuit { Code = "CIR1", CircuitName = "C", Locality = "LocC", Latitude = 0, Longitude = 0, CountryId =  country.Id };
        await repo.AddCircuitAsync(circuit);

        var race = new Race { RaceName = "NameR",RaceDate = DateOnly.FromDateTime(DateTime.UtcNow), Circuit = circuit, Season = season };
        await repo.AddRaceAsync(race);

        var driver = new Driver { Code = "RD1", GivenName = "X", FamilyName = "Y" , CountryId =  country.Id };
        await repo.AddDriverAsync(driver);

        var re = new RaceEntry { Race = race, Driver = driver, Position = 1, Grid = 1, Finished = true };
        var addedList = await repo.AddListRaceEntriesAsync(new List<RaceEntry> { re });

        Assert.Single(addedList);
        var dates = await repo.GetAllRaceDatesAsync();
        Assert.Contains(race.RaceDate, dates);
    }

    [Fact]
    public async Task GetRaceEntryByDriverCodeAndCircuitCodeInYearAsync_ReturnsEntry()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);
        
        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = new List<string> { "Thai" } };
        await repo.AddCountryAsync(country);
        
        var season = new Season { Year = 1999 };
        await repo.AddSeasonAsync(season);

        var circuit = new Circuit { Code = "CIRX", CircuitName = "C",Locality = "LOC", Country = country};
        await repo.AddCircuitAsync(circuit);

        var race = new Race { RaceName = "Name",RaceDate = DateOnly.Parse("1999-07-11"), Circuit = circuit, Season = season };
        await repo.AddRaceAsync(race);

        var driver = new Driver { Code = "DRX", GivenName = "G", FamilyName = "F", Country = country };
        await repo.AddDriverAsync(driver);

        var re = new RaceEntry { Race = race, Driver = driver, Position = 3, Grid = 2, Finished = true };
        await repo.AddRaceEntryAsync(re);

        var found = await repo.GetRaceEntryByDriverCodeAndCircuitCodeInYearAsync("DRX", "CIRX", 1999);
        Assert.NotNull(found);
        Assert.Equal(driver.Id, found.DriverId);
    }

    [Fact]
    public async Task AddPickableItemAsync_GetPickableItemAsync_Works()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);

        var item = await repo.AddPickableItemAsync();
        Assert.NotNull(item);
        var fetched = await repo.GetPickableItemAsync();
        Assert.NotNull(fetched);
        Assert.Equal(item.Id, fetched.Id);
    }

    [Fact]
    public async Task GetCounts_ReturnsCorrectCounts()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new DataSyncRepository(ctx);
        
        var country = new Country { Id = "THA", ShortName = "Thailand", Nationalities = new List<string> { "Thai" } };
        await repo.AddCountryAsync(country);
        
        // seed minimal data
        await repo.AddDriverAsync(new Driver { FamilyName = "FName", GivenName = "GName", Code = "CNT1", Country = country});
        await repo.AddConstructorAsync(new Constructor { Code = "CC1", Name = "N", Country = country });
        await repo.AddCircuitAsync(new Circuit { Code = "CT1", CircuitName = "N", Locality = "LOC",Country = country });
        await repo.AddSeasonAsync(new Season { Year = 2023 });
        await repo.AddRaceAsync(new Race { RaceName = "RName",RaceDate = DateOnly.FromDateTime(DateTime.UtcNow), SeasonId = 1, CircuitId = 1 });

        var driversCount = await repo.GetDriversCountAsync();
        var ctorsCount = await repo.GetConstructorsCountAsync();
        var circuitsCount = await repo.GetCircuitsCountAsync();
        var racesCount = await repo.GetRacesCountAsync();
        var seasonsCount = await repo.GetSeasonsCountAsync();

        Assert.True(driversCount >= 1);
        Assert.True(ctorsCount >= 1);
        Assert.True(circuitsCount >= 1);
        Assert.True(racesCount >= 1);
        Assert.True(seasonsCount >= 1);
    }
}