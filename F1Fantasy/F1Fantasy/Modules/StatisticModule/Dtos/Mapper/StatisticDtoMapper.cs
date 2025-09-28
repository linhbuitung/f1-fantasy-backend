using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.StatisticModule.Dtos.Mapper;

public class StatisticDtoMapper
{
    public static Get.GeneralSeasonStatisticDto MapToGeneralSeasonStatisticDto(int highestPoint, int totalTransferMade, string mostPickedDriver, double averagePoints)
    {
        return new Get.GeneralSeasonStatisticDto
        {
            HighestPoint = highestPoint,
            TotalTransferMade = totalTransferMade,
            MostPickedDriver = mostPickedDriver,
            AveragePoints = averagePoints
        };
    }
    
    public static Get.UserGeneralSeasonStatisticDto MapToUserGeneralSeasonStatisticDto(
        FantasyLineup fantasyLineup, 
        int totalPointsGained, 
        int totalTransferMade,
        int overallRank = 0)
    {
        return new Get.UserGeneralSeasonStatisticDto
        {
            TotalPointsGained = totalPointsGained,
            TotalTransfersMade = totalTransferMade,
            OverallRank = overallRank,
            BestRaceWeek = new Get.BestRaceWeekOfAnUserDto{
                FantasyLineupId = fantasyLineup.Id,
                RaceName = fantasyLineup.Race.RaceName,
                PointsGained = fantasyLineup.PointsGained,
                RaceDate = fantasyLineup.Race.RaceDate
            }
        };
    }

    public static Get.TeamOfTheRaceDto MapToTeamOfTheRaceDto(int raceId, string raceName, int round, List<RaceEntry> driverEntries,
        List<RaceEntry> constructorEntries)
    {
        return new Get.TeamOfTheRaceDto
        {
            Id = raceId,
            RaceName = raceName,
            Round = round,
            Drivers = driverEntries.Select(re => new Get.DriverInTeamOfTheRaceDto
            {
                Id = re.DriverId,
                Name = String.Concat(re.Driver.GivenName, " ", re.Driver.FamilyName),
                PointGained = re.PointsGained,
                ImgUrl = re.Driver.ImgUrl

            }).ToList(),
            Constructors = constructorEntries.Select(re => new Get.ConstructorInTeamOfTheRaceDto
            {
                Id = re.ConstructorId,
                Name =re.Constructor.Name,
                PointGained = re.PointsGained,
                ImgUrl = re.Constructor.ImgUrl
            }).ToList()
        };
    }
}