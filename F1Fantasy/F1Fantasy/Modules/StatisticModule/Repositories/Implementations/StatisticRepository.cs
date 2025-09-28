using System.Text;
using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.StatisticModule.Repositories.Implementations;

public class StatisticRepository(WooF1Context context) : IStatisticRepository
{
    public async Task<int> GetHighestScoreBySeasonIdAsync(int seasonId)
    {
        // Score from each user from the whole season
        return await context.FantasyLineups
            .Where(fl => fl.Race.SeasonId == seasonId)
            .GroupBy(fl => fl.UserId)
            .Select(g => g.Sum(fl => fl.PointsGained))
            .MaxAsync();    
    }

    public async Task<double> GetAverageScoreBySeasonIdAsync(int seasonId)
    {
        // Average core from all user from the whole season
        return await context.FantasyLineups
            .Where(fl => fl.Race.SeasonId == seasonId 
                         && fl.Race.DeadlineDate > fl.User.JoinDate)
            .GroupBy(fl => fl.UserId)
            .Select(g => g.Sum(fl => fl.PointsGained))
            .AverageAsync();
    }
    

    public async Task<string> GetMostPickedDriverAsync(int seasonId)
    {
        var mostPickedDriverId = await context.FantasyLineupDrivers
            .Where(fld => fld.FantasyLineup.Race.SeasonId == seasonId)
            .GroupBy(fld => fld.DriverId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();
        
        var mostPickedDriver =await context.Drivers.FirstOrDefaultAsync(d => d.Id == mostPickedDriverId);

        if (mostPickedDriver == null)
        {
            return "N/A";
        }
        return String.Concat(mostPickedDriver.GivenName, " ", mostPickedDriver.FamilyName);
    }

    public async Task<int> GetTotalTransfersBySeasonIdAsync(int seasonId)
    {
        return await context.FantasyLineups
            .Where(fl => fl.Race.SeasonId == seasonId)
            .SumAsync(fl => fl.TransfersMade);
    }

    public async Task<FantasyLineup?> GetBestFantasyLineupOfAnUserBySeasonIdAsync(int userId, int seasonId)
    {
        return await context.FantasyLineups
            .Include(fl => fl.Race)
            .AsNoTracking()
            .Where(fl => fl.Race.SeasonId == seasonId && fl.UserId == userId && fl.Race.Calculated)
            .OrderByDescending(fl => fl.PointsGained)
            .ThenByDescending(fl => fl.Race.RaceDate)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetTotalPointOfAnUserBySeasonIdAsync(int userId, int seasonId)
    {
        return await context.FantasyLineups
            .Where(fl => fl.Race.SeasonId == seasonId && fl.UserId == userId)
            .SumAsync(fl => fl.PointsGained);
    }

    public async Task<int> GetTotalTransfersOfAnUserBySeasonIdAsync(int userId, int seasonId)
    {
        return await context.FantasyLineups
            .Where(fl => fl.UserId == userId && fl.Race.SeasonId == seasonId)
            .SumAsync(fl => fl.TransfersMade);
    }

    public async Task<int> GetOverallRankOfAnUserBySeasonIdAsync(int userId, int seasonId)
    {
        var userScores = await context.FantasyLineups
            .Where(fl => fl.Race.SeasonId == seasonId)
            .GroupBy(fl => fl.UserId)
            .Select(g => new { UserId = g.Key, TotalPoints = g.Sum(fl => fl.PointsGained) })
            .OrderByDescending(x => x.TotalPoints)
            .ToListAsync();

        var rank = userScores.FindIndex(x => x.UserId == userId) + 1;
        return rank > 0 ? rank : -1;
    }
    
    public async Task<List<RaceEntry>> GetTopDriverRaceEntriesInARaceByRaceIdAsync(int raceId, int topN)
    {
        return await context.RaceEntries
            .Include(re => re.Driver)
            .AsNoTracking()
            .Where(re => re.RaceId == raceId && re.Finished)
            .OrderByDescending(re => re.PointsGained)
            .Take(topN)
            .ToListAsync();
    }

    public async Task<List<RaceEntry>> GetTopConstructorRaceEntriesInARaceByRaceIdAsync(int raceId, int topN)
    {
        return await context.RaceEntries
            .Include(re => re.Constructor)
            .AsNoTracking()
            .Where(re => re.RaceId == raceId && re.Finished)
            .OrderByDescending(re => re.PointsGained)
            .Take(topN)
            .ToListAsync();
    }
}