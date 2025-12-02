using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StatisticModule.Repositories.Implementations;
using F1Fantasy.Core.Common;

namespace Tests.ApiTests;

public class StatisticRepositoryTests
{
    private WooF1Context CreateInMemoryContext(string name = null)
    {
        var options = new DbContextOptionsBuilder<WooF1Context>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .Options;
        return new WooF1Context(options);
    }

    [Fact]
    public async Task GetHighestScoreBySeasonIdAsync_ReturnsMaxUserScore()
    {
        await using var ctx = CreateInMemoryContext();
        
        // Seed data
        var season = new Season { Id = 1, Year = 2024 };
        var user1 = new ApplicationUser { Id = 1, UserName = "user1", Email = "user1@test.com", JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)) };
        var user2 = new ApplicationUser { Id = 2, UserName = "user2", Email = "user2@test.com", JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)) };
        
        ctx.Seasons.Add(season);
        ctx.Users.AddRange(user1, user2);
        await ctx.SaveChangesAsync();

        var circuit = new Circuit { Id = 1, Code = "TEST", CircuitName = "Test", Locality = "Loc", CountryId = "GB" };
        var country = new Country { Id = "GB", ShortName = "Britain", Nationalities = ["British"] };
        ctx.Countries.Add(country);
        ctx.Circuits.Add(circuit);
        
        var race1 = new Race { Id = 1, RaceName = "R1",SeasonId = season.Id, CircuitId = circuit.Id, RaceDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        var race2 = new Race { Id = 2, RaceName = "R2", SeasonId = season.Id, CircuitId = circuit.Id, RaceDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)) };
        ctx.Races.AddRange(race1, race2);
        await ctx.SaveChangesAsync();

        // User1: 100 + 150 = 250 total
        // User2: 80 + 90 = 170 total
        ctx.FantasyLineups.AddRange(
            new FantasyLineup { UserId = user1.Id, RaceId = race1.Id, TotalAmount = 100, TransfersMade = 0, PointsGained = 0 },
            new FantasyLineup { UserId = user1.Id, RaceId = race2.Id, TotalAmount = 150, TransfersMade = 0, PointsGained = 0 },
            new FantasyLineup { UserId = user2.Id, RaceId = race1.Id, TotalAmount = 80, TransfersMade = 0, PointsGained = 0 },
            new FantasyLineup { UserId = user2.Id, RaceId = race2.Id, TotalAmount = 90, TransfersMade = 0, PointsGained = 0 }
        );
        await ctx.SaveChangesAsync();

        var repo = new StatisticRepository(ctx);
        var highest = await repo.GetHighestScoreBySeasonIdAsync(season.Id);

        Assert.Equal(250, highest);
    }

    [Fact]
    public async Task GetAverageScoreBySeasonIdAsync_CalculatesCorrectly()
    {
        await using var ctx = CreateInMemoryContext();
        
        var season = new Season { Id = 1, Year = 2024 };
        var user1 = new ApplicationUser { Id = 1, UserName = "user1", Email = "user1@test.com", JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)) };
        var user2 = new ApplicationUser { Id = 2, UserName = "user2", Email = "user2@test.com", JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)) };
        
        ctx.Seasons.Add(season);
        ctx.Users.AddRange(user1, user2);
        await ctx.SaveChangesAsync();

        var circuit = new Circuit { Id = 1, Code = "TEST", CircuitName = "Test", Locality = "Loc", CountryId = "GB" };
        var country = new Country { Id = "GB", ShortName = "Britain", Nationalities = ["British"] };
        ctx.Countries.Add(country);
        ctx.Circuits.Add(circuit);
        
        var race1 = new Race { Id = 1, RaceName = "R1", SeasonId = season.Id, CircuitId = circuit.Id, RaceDate = DateOnly.FromDateTime(DateTime.UtcNow), DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)) };
        ctx.Races.Add(race1);
        await ctx.SaveChangesAsync();

        // User1: 200 total, User2: 100 total -> avg = 150
        ctx.FantasyLineups.AddRange(
            new FantasyLineup { UserId = user1.Id, RaceId = race1.Id, TotalAmount = 200, TransfersMade = 0, PointsGained = 0 },
            new FantasyLineup { UserId = user2.Id, RaceId = race1.Id, TotalAmount = 100, TransfersMade = 0, PointsGained = 0 }
        );
        await ctx.SaveChangesAsync();

        var repo = new StatisticRepository(ctx);
        var avg = await repo.GetAverageScoreBySeasonIdAsync(season.Id);

        Assert.Equal(150.0, avg);
    }

    [Fact]
    public async Task GetTotalPointOfAnUserBySeasonIdAsync_SumsUserLineups()
    {
        await using var ctx = CreateInMemoryContext();
        
        var season = new Season { Id = 1, Year = 2024 };
        var user = new ApplicationUser { Id = 1, UserName = "user1", Email = "user1@test.com", JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)) };
        
        ctx.Seasons.Add(season);
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var circuit = new Circuit { Id = 1, Code = "TEST", CircuitName = "Test", Locality = "Loc", CountryId = "GB" };
        var country = new Country { Id = "GB", ShortName = "Britain", Nationalities = ["British"] };
        ctx.Countries.Add(country);
        ctx.Circuits.Add(circuit);
        
        var race1 = new Race { Id = 1, RaceName = "R1", SeasonId = season.Id, CircuitId = circuit.Id, RaceDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        var race2 = new Race { Id = 2, RaceName = "R2", SeasonId = season.Id, CircuitId = circuit.Id, RaceDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)) };
        ctx.Races.AddRange(race1, race2);
        await ctx.SaveChangesAsync();

        ctx.FantasyLineups.AddRange(
            new FantasyLineup { UserId = user.Id, RaceId = race1.Id, TotalAmount = 50, TransfersMade = 0, PointsGained = 0 },
            new FantasyLineup { UserId = user.Id, RaceId = race2.Id, TotalAmount = 75, TransfersMade = 0, PointsGained = 0 }
        );
        await ctx.SaveChangesAsync();

        var repo = new StatisticRepository(ctx);
        var total = await repo.GetTotalPointOfAnUserBySeasonIdAsync(user.Id, season.Id);

        Assert.Equal(125, total);
    }

    [Fact]
    public async Task GetTotalTransfersBySeasonIdAsync_CountsAllTransfers()
    {
        await using var ctx = CreateInMemoryContext();
        
        var season = new Season { Id = 1, Year = 2024 };
        var user1 = new ApplicationUser { Id = 1, UserName = "user1", Email = "user1@test.com", JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)) };
        var user2 = new ApplicationUser { Id = 2, UserName = "user2", Email = "user2@test.com", JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)) };
        
        ctx.Seasons.Add(season);
        ctx.Users.AddRange(user1, user2);
        await ctx.SaveChangesAsync();

        var circuit = new Circuit { Id = 1, Code = "TEST", CircuitName = "Test", Locality = "Loc", CountryId = "GB" };
        var country = new Country { Id = "GB", ShortName = "Britain", Nationalities = ["British"] };
        ctx.Countries.Add(country);
        ctx.Circuits.Add(circuit);
        
        var race = new Race { Id = 1, RaceName = "R1", SeasonId = season.Id, CircuitId = circuit.Id, RaceDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        ctx.Races.Add(race);
        await ctx.SaveChangesAsync();

        ctx.FantasyLineups.AddRange(
            new FantasyLineup { UserId = user1.Id, RaceId = race.Id, TotalAmount = 0, TransfersMade = 2, PointsGained = 0 },
            new FantasyLineup { UserId = user2.Id, RaceId = race.Id, TotalAmount = 0, TransfersMade = 3, PointsGained = 0 }
        );
        await ctx.SaveChangesAsync();

        var repo = new StatisticRepository(ctx);
        var totalTransfers = await repo.GetTotalTransfersBySeasonIdAsync(season.Id);

        Assert.Equal(5, totalTransfers);
    }
}