using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.CoreGameplayModule.Repositories.Implementations;
using F1Fantasy.Core.Common;

namespace Tests.ApiTests;

public class CoreGameplayRepositoryTests
{
    private WooF1Context CreateInMemoryContext(string name = null)
    {
        var options = new DbContextOptionsBuilder<WooF1Context>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .Options;
        return new WooF1Context(options);
    }

    [Fact]
    public async Task GetFantasyLineupByUserIdAndRaceIdAsync_ReturnsLineup()
    {
        await using var ctx = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "test@test.com",
            Email = "test@test.com",
            JoinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30))
        };
        ctx.Users.Add(user);

        var season = new Season { Id = 1, Year = 2024 };
        ctx.Seasons.Add(season);

        var country = new Country { Id = "GB", ShortName = "Britain", Nationalities = ["British"] };
        ctx.Countries.Add(country);

        var circuit = new Circuit { Id = 1, Code = "SPA", CircuitName = "Spa",Locality = "Loc", CountryId = country.Id };
        ctx.Circuits.Add(circuit);

        var race = new Race
        {
            Id = 1,
            SeasonId = season.Id,
            CircuitId = circuit.Id,
            RaceDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        ctx.Races.Add(race);

        var lineup = new FantasyLineup
        {
            UserId = user.Id,
            RaceId = race.Id,
            TotalAmount = 100,
            TransfersMade = 0,
            PointsGained = 0
        };
        ctx.FantasyLineups.Add(lineup);
        await ctx.SaveChangesAsync();

        var repo = new FantasyLineupRepository(ctx );
        var result = await repo.GetFantasyLineupByUserIdAndRaceIdAsync(user.Id, race.Id);

        Assert.NotNull(result);
        Assert.Equal(100, result.TotalAmount);
    }
}