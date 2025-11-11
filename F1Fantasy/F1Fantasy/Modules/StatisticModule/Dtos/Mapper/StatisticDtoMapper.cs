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

    #region  driver statistics dto mappers

    public static Get.DriverWithTotalFantasyPointScoredGetDto MapToDriverWithTotalFantasyPointScoredGetDto(Driver driver, int totalFantasyPointScored)
    {
        return new Get.DriverWithTotalFantasyPointScoredGetDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            TotalFantasyPointScored = totalFantasyPointScored,
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithAveragePointScoredGetDto MapToDriverWithAverageFantasyPointScoredGetDto(Driver driver, double averageFantasyPointScored)
    {
        return new Get.DriverWithAveragePointScoredGetDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            AverageFantasyPointScored = Math.Round(averageFantasyPointScored, 2),
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithRaceWinsGetDto MapToDriverWithRaceWinsGetDto(Driver driver, int totalRacesWin)
    {
        return new Get.DriverWithRaceWinsGetDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            TotalRacesWin = totalRacesWin,
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithPodiumsGetDto MapToDriverWithPodiumsGetDto(Driver driver,int totalPodiums)
    {
        return new Get.DriverWithPodiumsGetDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            TotalPodiums = totalPodiums,
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithTop10FinishesGetDto MapToDriverWithTop10FinishesGetDto(Driver driver,int totalTop10Finishes)
    {
        return new Get.DriverWithTop10FinishesGetDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            TotalTop10Finishes = totalTop10Finishes,
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithFastestLapsGetDto MapToDriverWithFastestLapsGetDto(Driver driver,int totalFastestLaps)
    {
        return new Get.DriverWithFastestLapsGetDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            TotalFastestLaps = totalFastestLaps,
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithDnfsGetDto MapToDriverWithDnfsGetDto(Driver driver,int totalDnfs)
    {
        return new Get.DriverWithDnfsGetDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            TotalDnfs = totalDnfs,
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }
    
    public static Get.DriverWithSelectionPercentageGetDto MapToDriverWithSelectionPercentageDto(Driver driver, double selectionPercentage)
    {
        return new Get.DriverWithSelectionPercentageGetDto
        {
            Id = driver.Id,
            GivenName = driver.GivenName,
            FamilyName = driver.FamilyName,
            SelectionPercentage = Math.Round(selectionPercentage, 2),
            Code = driver.Code,
            Price = driver.Price,
            ImgUrl = driver.ImgUrl
        };
    }

    #endregion

    #region  constructor statistics dto mappers
    public static Get.ConstructorWithTotalFantasyPointScoredGetDto MapToConstructorWithTotalFantasyPointScoredGetDto(Constructor constructor, int totalFantasyPointScored)
    {
        return new Get.ConstructorWithTotalFantasyPointScoredGetDto
        {
            Id = constructor.Id,
            Name = constructor.Name,
            TotalFantasyPointScored = totalFantasyPointScored,
            Code = constructor.Code,
            Price = constructor.Price,
            ImgUrl = constructor.ImgUrl
        };
    }
    
    public static Get.ConstructorWithAveragePointScoredGetDto MapToConstructorWithAverageFantasyPointScoredGetDto(Constructor constructor, double averageFantasyPointScored)
    {
        return new Get.ConstructorWithAveragePointScoredGetDto
        {
            Id = constructor.Id,
            Name = constructor.Name,
            AverageFantasyPointScored = Math.Round(averageFantasyPointScored, 2),
            Code = constructor.Code,
            Price = constructor.Price,
            ImgUrl = constructor.ImgUrl
        };
    }
    
    public static Get.ConstructorWithSelectionPercentageGetDto MapToConstructorWithSelectionPercentageGetDto(Constructor constructor, double selectionPercentage)
    {
        return new Get.ConstructorWithSelectionPercentageGetDto
        {
            Id = constructor.Id,
            Name = constructor.Name,
            SelectionPercentage = Math.Round(selectionPercentage, 2),
            Code = constructor.Code,
            Price = constructor.Price,
            ImgUrl = constructor.ImgUrl
        };
    }
    
    public static Get.ConstructorWithPodiumsGetDto MapToConstructorWithPodiumsGetDto(Constructor constructor,int totalPodiums)
    {
        return new Get.ConstructorWithPodiumsGetDto
        {
            Id = constructor.Id,
            Name = constructor.Name,
            TotalPodiums = totalPodiums,
            Code = constructor.Code,
            Price = constructor.Price,
            ImgUrl = constructor.ImgUrl
        };
    }
    
    public static Get.ConstructorWithTop10FinishesGetDto MapToConstructorWithTop10FinishesGetDto(Constructor constructor,int totalTop10Finishes)
    {
        return new Get.ConstructorWithTop10FinishesGetDto
        {
            Id = constructor.Id,
            Name = constructor.Name,
            TotalTop10Finishes = totalTop10Finishes,
            Code = constructor.Code,
            Price = constructor.Price,
            ImgUrl = constructor.ImgUrl
        };
    }
    
    #endregion
    
    #region  user statistics dto mappers
    
    public static Get.UserWithTotalFantasyPointScoredGetDto MapToUserWithTotalFantasyPointScoredGetDto(ApplicationUser user, int totalFantasyPointScored)
    {
        return new Get.UserWithTotalFantasyPointScoredGetDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            JoinDate = user.JoinDate,
            TotalFantasyPointScored = totalFantasyPointScored
        };
    }
    
    public static Get.UserWithAveragePointScoredGetDto MapToUserWithAveragePointScoredGetDto(ApplicationUser user, double averageFantasyPointScored)
    {
        return new Get.UserWithAveragePointScoredGetDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            JoinDate = user.JoinDate,
            AverageFantasyPointScored = Math.Round(averageFantasyPointScored, 2)
        };
    }
    
    #endregion

    #region race statistics dto mappers
    
        public static Get.RaceStatisticDto MapToInitialRaceStatisticDto(Race race)
        {
            // Leave out constructor statistic to be calculated later
            return new Get.RaceStatisticDto
            {
                Id = race.Id,
                RaceName = race.RaceName,
                Round = race.Round,
                RaceDate = race.RaceDate,
                DeadlineDate = race.DeadlineDate,
                Calculated = race.Calculated,
                Circuit = MapToCircuitInRaceStatisticDto(race.Circuit),
                DriversStatistics = race.RaceEntries.Select(MapToDriverInRaceStatisticDto).ToList(),
            };
        }
        
        public static Get.CircuitInRaceStatisticDto MapToCircuitInRaceStatisticDto(Circuit circuit)
        {
            return new Get.CircuitInRaceStatisticDto
            {
                CircuitName = circuit.CircuitName,
                Code = circuit.Code,
                Country = circuit.Country.ShortName,
                ImgUrl = circuit.ImgUrl,
            };
        }

        public static Get.DriverInRaceStatisticDto MapToDriverInRaceStatisticDto(RaceEntry raceEntry)
        {
            return new Get.DriverInRaceStatisticDto
            {
                Id = raceEntry.Driver.Id,
                GivenName = raceEntry.Driver.GivenName,
                FamilyName = raceEntry.Driver.FamilyName,
                ConstructorName = raceEntry.Constructor.Name,
                ImgUrl = raceEntry.Driver.ImgUrl,
                Position = raceEntry.Position,
                Grid = raceEntry.Grid,
                FastestLap = raceEntry.FastestLap,
                PointsGained = raceEntry.PointsGained,
                Finished = raceEntry.Finished,
            } ;
        }

        public static Get.ConstructorInRaceStatisticDto MapToConstructorInRaceStatisticDto(Constructor constructor,
            int pointsGained)
        {
            return new Get.ConstructorInRaceStatisticDto
            {
                Id = constructor.Id,
                Name = constructor.Name,
                ImgUrl = constructor.ImgUrl,
                PointsGained = pointsGained
            };
        }


        #endregion
}