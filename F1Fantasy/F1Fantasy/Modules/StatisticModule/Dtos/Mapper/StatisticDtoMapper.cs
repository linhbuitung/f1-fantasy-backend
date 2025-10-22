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
    
    public static Get.DriverWithTotalFantasyPointScoredGetDto MapToDriverWithTotalFantasyPointScoredDto(Driver driver, int totalFantasyPointScored)
    {
        return new Get.DriverWithTotalFantasyPointScoredGetDto
        {
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            TotalFantasyPointScored = totalFantasyPointScored,
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithAveragePointScoredGetDto MapToDriverWithAverageFantasyPointScoredDto(Driver driver, double averageFantasyPointScored)
    {
        return new Get.DriverWithAveragePointScoredGetDto
        {
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            AverageFantasyPointScored = Math.Round(averageFantasyPointScored, 2),
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithSelectionPercentageGetDto MapToDriverWithSelectionPercentageDto(Driver driver, double selectionPercentage)
    {
        return new Get.DriverWithSelectionPercentageGetDto
        {
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            SelectionPercentage = Math.Round(selectionPercentage, 2),
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
}