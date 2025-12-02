using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using F1Fantasy;
using Microsoft.Extensions.DependencyInjection;
using F1Fantasy.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using F1Fantasy.Core.Common;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Tests.ApiTests;

public class StatisticControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StatisticControllerTests(WebApplicationFactory<Program> factory)
    {
        // SET ENVIRONMENT VARIABLE BEFORE FACTORY INITIALIZATION
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;",
                }!);
            });

            builder.ConfigureServices(services =>
            {
                // Remove real DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<WooF1Context>));
                if (descriptor != null) services.Remove(descriptor);

                // Add in-memory DB
                services.AddDbContext<WooF1Context>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });

                // Seed test data
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<WooF1Context>();
                db.Database.EnsureCreated();
                SeedTestData(db);
            });
        });
    }    
    private static void SeedTestData(WooF1Context db)
    {
        // Seed a season
        var season = new Season { Id = 1, Year = 2024, IsActive = true };
        db.Seasons.Add(season);
        
        // Seed users
        var user1 = new ApplicationUser
        {
            Id = 1,
            UserName = "test1@test.com",
            Email = "test1@test.com",
            JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30))
        };
        var user2 = new ApplicationUser
        {
            Id = 2,
            UserName = "test2@test.com",
            Email = "test2@test.com",
            JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30))
        };
        db.Users.AddRange(user1, user2);
        
        // Seed circuit and country
        var country = new Country { Id = "GB", ShortName = "Britain", Nationalities = ["British"] };
        db.Countries.Add(country);
        
        var circuit = new Circuit { Id = 1, Code = "SPA", CircuitName = "Spa", Locality = "Loc",CountryId = country.Id };
        db.Circuits.Add(circuit);
        
        // Seed race
        var race = new Race
        {
            Id = 1,
            SeasonId = season.Id,
            CircuitId = circuit.Id,
            RaceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
        };
        db.Races.Add(race);
        
        // Seed fantasy lineups with points
        db.FantasyLineups.AddRange(
            new FantasyLineup
            {
                UserId = user1.Id,
                RaceId = race.Id,
                TotalAmount = 200,
                TransfersMade = 0,
                PointsGained = 0
            },
            new FantasyLineup
            {
                UserId = user2.Id,
                RaceId = race.Id,
                TotalAmount = 150,
                TransfersMade = 1,
                PointsGained = 0
            }
        );

        db.SaveChanges();
    }

    /*
    [Fact]
    public async Task GetGeneralStatisticForCurrentActiveSeason_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/statistic/general/current-season");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }*/
}