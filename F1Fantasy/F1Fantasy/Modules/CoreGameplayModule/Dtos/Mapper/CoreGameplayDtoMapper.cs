using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.CoreGameplayModule.Dtos.Mapper;

public class CoreGameplayDtoMapper
{
    public static Get.FantasyLineupDto MapFantasyLineupToGetDto(FantasyLineup fantasyLineup)
    {
        return new Get.FantasyLineupDto
        {
            Id = fantasyLineup.Id,
            TotalAmount = fantasyLineup.TotalAmount,
            TransfersMade = fantasyLineup.TransfersMade,
            PointsGained = fantasyLineup.PointsGained,
            UserId = fantasyLineup.UserId,
            RaceId = fantasyLineup.RaceId,
            Drivers = fantasyLineup.FantasyLineupDrivers.Select(fld => fld.Driver)
                          .Select(d => new Get.DriverInFantasyLineupDto
                          {
                              Id = d.Id,
                              GivenName = d.GivenName,
                              FamilyName = d.FamilyName,
                              DateOfBirth = d.DateOfBirth,
                              CountryId = d.CountryId,
                              Code = d.Code,
                              Price = d.Price,
                              ImgUrl = d.ImgUrl
                          })
                          .ToList(),
            Constructors = fantasyLineup.FantasyLineupConstructors.Select(flc => flc.Constructor)
                .Select(c => new Get.ConstructorInFantasyLineupDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Price = c.Price,
                    ImgUrl = c.ImgUrl,
                    CountryId = c.CountryId
                })
                .ToList(),
            Powerups = fantasyLineup.PowerupFantasyLineups.Select(flp => flp.Powerup)
                .Select(p => new Get.PowerupInFantasyLineupDto
                {
                    Id = p.Id,
                    Type = p.Type,
                    Description = p.Description,
                    ImgUrl = p.ImgUrl
                }).ToList()
        };
    }
    
    public static Get.RaceDto MapRaceToDto(Race race)
    {
        return new Get.RaceDto
        {
            Id = race.Id,
            RaceName = race.RaceName,
            Round = race.Round,
            RaceDate = race.RaceDate,
            DeadlineDate = race.DeadlineDate,
            Calculated = race.Calculated,
            SeasonYear = race.Season.Year,
            CircuitId = race.CircuitId,
            CircuitCode = race.Circuit.Code,
            CircuitName = race.Circuit.CircuitName
        };
    }

    public static Get.RaceResultDto MapRaceResultToDto(Race race)
    {
        return new Get.RaceResultDto
        { 
            Race = {
            Id = race.Id,
            RaceName = race.RaceName,
            Round = race.Round,
            RaceDate = race.RaceDate,
            DeadlineDate = race.DeadlineDate,
            Calculated = race.Calculated,
            SeasonYear = race.Season.Year,
            CircuitId = race.CircuitId,
            CircuitCode = race.Circuit.Code,
            CircuitName = race.Circuit.CircuitName
            },
            DriverResults = race.RaceEntries.Select(re => new Get.DriverRaceResultDto
            {
                Id = re.Driver.Id,
                GivenName = re.Driver.GivenName,
                FamilyName = re.Driver.FamilyName,
                Code = re.Driver.Code,
                PointGained = re.PointsGained,
                ImgUrl = re.Driver.ImgUrl
            }).ToList(),
            ConstructorResults = race.RaceEntries.Select(re=> new Get.ConstructorRaceResultDto
            {
                Id = re.Constructor.Id,
                Name = re.Constructor.Name,
                Code = re.Constructor.Code,
                PointGained = re.PointsGained,
                ImgUrl = re.Constructor.ImgUrl
            }).ToList()
        };
        
    }
}