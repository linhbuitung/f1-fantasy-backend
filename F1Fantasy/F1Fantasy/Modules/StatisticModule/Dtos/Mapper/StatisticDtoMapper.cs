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
            AveragePoints = Math.Round(averagePoints, 2),
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
                TotalAmount = fantasyLineup.TotalAmount,
                RaceDate = fantasyLineup.Race.RaceDate
            }
        };
    }

    public static Get.TeamOfTheRaceDto MapToTeamOfTheRaceDto(int raceId, string raceName, int round, List<Get.DriverInTeamOfTheRaceDto> driverEntries,
        List<Get.ConstructorInTeamOfTheRaceDto> constructorEntries)
    {
        return new Get.TeamOfTheRaceDto
        {
            Id = raceId,
            RaceName = raceName,
            Round = round,
            Drivers = driverEntries,
            Constructors = constructorEntries
        };
    }
}