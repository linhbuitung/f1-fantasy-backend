using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Dtos.Mapper;

public class CoreGameplayDtoMapper
{
    public static PowerupForPointApplicationDto MapToPowerupForPointApplicationDto(PowerupDto powerupDto, Powerup powerup )
    {
        if (powerupDto.Id != powerup.Id)
        {
            throw new ArgumentException("Invalid Powerup Id provided");
        }
        return new PowerupForPointApplicationDto
        {
            PowerupId = powerup.Id,
            Type = powerupDto.Type,
        };
    }
}