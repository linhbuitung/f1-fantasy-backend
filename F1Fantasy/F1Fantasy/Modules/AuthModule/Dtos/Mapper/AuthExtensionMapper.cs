using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.AuthModule.Dtos.Mapper;

public class AuthExtensionMapper
{
    public static FantasyLineup MapCreateDtoToFantasyLineup(Dtos.Create.FantasyLineupDto dto)
    {
        return new FantasyLineup
        {
            UserId = dto.UserId,
            RaceId = dto.RaceId,
            TotalAmount = 0,
            TransfersMade = 0,
            PointsGained = 0,
        };
    }
}