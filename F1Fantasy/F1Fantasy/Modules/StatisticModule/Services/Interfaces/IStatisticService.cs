using F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

namespace F1Fantasy.Modules.StatisticModule.Services.Interfaces;

public interface IStatisticService
{
    Task<Dtos.Get.GeneralSeasonStatisticDto> GetGeneralStatisticBySeasonId(int seasonId);

    Task<Dtos.Get.UserGeneralSeasonStatisticDto> GetUserGeneralStatisticByUserIdAndSeasonIdAsync(int userId,
        int seasonId);

    Task<Dtos.Get.TeamOfTheRaceDto> GetTeamOfTheRaceByRaceIdAsync(RaceDto raceDto);
}