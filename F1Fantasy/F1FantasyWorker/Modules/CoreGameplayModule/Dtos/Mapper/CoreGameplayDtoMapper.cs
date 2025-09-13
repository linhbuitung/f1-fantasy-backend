using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Dtos.Mapper;

public class CoreGameplayDtoMapper
{
    public static PowerupForPointApplicationDto MapToPowerupForPointApplicationDto(PowerupDto powerupDto, PowerupFantasyLineup powerupFantasyLineup )
    {
        if (powerupDto.Id != powerupFantasyLineup.PowerupId)
        {
            throw new ArgumentException("Invalid Powerup Id provided");
        }
        return new PowerupForPointApplicationDto
        {
            PowerupId = powerupFantasyLineup.PowerupId,
            Type = powerupDto.Type,
            // Set driver id if the powerup type is 'DRS Enabled'
            DriverId = powerupDto.Type == "DRS Enabled" ? powerupFantasyLineup.DriverId : null
        };
    }
}