using F1Fantasy.Exceptions;
using F1Fantasy.Modules.AdminModule.Repositories.Interfaces;
using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;
using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Dtos.Mapper;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;

namespace F1Fantasy.Modules.StatisticModule.Services.Implementations;

public class StatisticService(IStatisticRepository statisticRepository, ICoreGameplayService coreGameplayService, IAdminRepository adminRepository) : IStatisticService
{
    public async Task<Dtos.Get.GeneralSeasonStatisticDto> GetGeneralStatisticBySeasonId(int seasonId)
    {
        var highestPoint = await statisticRepository.GetHighestScoreBySeasonIdAsync(seasonId);
        var averagePoints = await statisticRepository.GetAverageScoreBySeasonIdAsync(seasonId);
        var mostPickedDriver = await statisticRepository.GetMostPickedDriverAsync(seasonId);
        var totalTransferMade = await statisticRepository.GetTotalTransfersBySeasonIdAsync(seasonId);
        
        return StatisticDtoMapper.MapToGeneralSeasonStatisticDto(highestPoint, totalTransferMade, mostPickedDriver, averagePoints);
    }

    public async Task<Dtos.Get.UserGeneralSeasonStatisticDto> GetUserGeneralStatisticByUserIdAndSeasonIdAsync(int userId,int seasonId)
    {
        var bestFantasyLineup = await statisticRepository.GetBestFantasyLineupOfAnUserBySeasonIdAsync(userId, seasonId);

        if (bestFantasyLineup == null)
        {
            throw new NotFoundException("No FantasyLineup found");
        }
        
        var totalPoints = await statisticRepository.GetTotalPointOfAnUserBySeasonIdAsync(userId, seasonId);
        var totalTransfersMade = await statisticRepository.GetTotalTransfersOfAnUserBySeasonIdAsync(userId, seasonId);
        var overallRank = await statisticRepository.GetOverallRankOfAnUserBySeasonIdAsync(userId, seasonId);
        return StatisticDtoMapper.MapToUserGeneralSeasonStatisticDto(bestFantasyLineup, totalPoints, totalTransfersMade, overallRank);
    }

    public async Task<Dtos.Get.TeamOfTheRaceDto> GetTeamOfTheRaceByRaceIdAsync( RaceDto raceDto)
    {
        var allRaceEntries = await statisticRepository.GetAllRaceEntriesByRaceIdAsync(raceDto.Id);
        var top5Drivers = allRaceEntries
            .OrderByDescending(re => re.PointsGained)
            .ThenBy(re => re.Position)
            .Take(5)
            .ToList();

        var constructorPoints = new Dictionary<int, int>();
        var constructorIdsInRace = allRaceEntries.Select(e => e.ConstructorId).Distinct().ToList();
        // create a dictionary with constructorId as key and 0 as value
        foreach (var constructorId in constructorIdsInRace)
        {
            constructorPoints[constructorId] = 0;
            var raceEntriesForConstructorPointCalculation = allRaceEntries
                .Where(e => e.ConstructorId == constructorId)
                .ToList();
                    
            var positions = raceEntriesForConstructorPointCalculation.Select(d => d.Position).ToList();

            if (positions.Count == 2)
            {
                bool bothTop3 = positions.All(p => p > 0 && p <= 3);
                bool oneTop3 = positions.Count(p => p > 0 && p <= 3) == 1;
                bool bothTop10 = positions.All(p => p > 0 && p <= 10);
                bool oneTop10 = positions.Count(p => p > 0 && p <= 10) == 1;

                if (bothTop3)
                    constructorPoints[constructorId] += 30;
                else if (oneTop3)
                    constructorPoints[constructorId] += 20;
                else if (bothTop10)
                    constructorPoints[constructorId] += 15;
                else if (oneTop10)
                    constructorPoints[constructorId] += 10;
                else
                    constructorPoints[constructorId] += -10;
            }
            else
            {
                // Handle missing drivers if needed
                constructorPoints[constructorId] = -10;
            }
        }
        // Get top 2 constructors based on points from constructorPoints dictionary, if tie, sort by constructorId 
        var top2ConstructorEntries = allRaceEntries
            .Where(re => constructorPoints.ContainsKey(re.ConstructorId))
            .GroupBy(re => re.ConstructorId)
            .Select(g => new
            {
                ConstructorId = g.Key,
                Name = g.First().Constructor.Name,
                PointGained = constructorPoints[g.Key],
                ImgUrl = g.First().Constructor.ImgUrl
            }).ToList();

        var driverDtos = top5Drivers.Select(re => new Dtos.Get.DriverInTeamOfTheRaceDto
        {
            Id = re.DriverId,
            Name = String.Concat(re.Driver.GivenName, " ", re.Driver.FamilyName),
            PointGained = re.PointsGained,
            ImgUrl = re.Driver.ImgUrl
        }).ToList();

        var constructorDtos = top2ConstructorEntries.Select(re => new Dtos.Get.ConstructorInTeamOfTheRaceDto
        {
            Id = re.ConstructorId,
            Name = re.Name,
            PointGained = re.PointGained,
            ImgUrl = re.ImgUrl
        })
        .OrderByDescending(dto => dto.PointGained)
        .Take(2)
        .ToList();
        
        return StatisticDtoMapper.MapToTeamOfTheRaceDto(raceDto.Id, raceDto.RaceName, raceDto.Round, driverDtos, constructorDtos);
    }

    public async Task<List<Dtos.Get.DriverWIthFantasyPointScored>> GetTopDriversInSeasonByFantasyPointsAsync(int seasonId)
    {
        var drivers = await statisticRepository.GetAllDriversIncludeRaceEntriesBySeasonIdAsync(seasonId);
        var driverPointsList = new List<Dtos.Get.DriverWIthFantasyPointScored>();

        foreach (var driver in drivers)
        {
            // Calculate total fantasy points for the driver in the season and create DTO
            
            // Repository already includes RaceEntries for the season so we can sum directly
            var totalPoints = driver.RaceEntries
                .Sum(re => re.PointsGained);

            var driverDto = StatisticDtoMapper.MapToDriverWIthFantasyPointScoredDto(driver, totalPoints);
            driverPointsList.Add(driverDto);
        }
        
        return driverPointsList
            .OrderByDescending(dp => dp.FantasyPointScored)
            .ToList();
    }
}