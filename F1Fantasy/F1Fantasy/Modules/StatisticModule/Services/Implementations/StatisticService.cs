using F1Fantasy.Exceptions;
using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;
using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Dtos.Mapper;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;

namespace F1Fantasy.Modules.StatisticModule.Services.Implementations;

public class StatisticService(IStatisticRepository statisticRepository, ICoreGameplayService coreGameplayService) : IStatisticService
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
        return StatisticDtoMapper.MapToUserGeneralSeasonStatisticDto(bestFantasyLineup, totalPoints);
    }

    public async Task<Dtos.Get.TeamOfTheRaceDto> GetTeamOfTheRaceByRaceIdAsync( RaceDto raceDto)
    {
        var top5Drivers = await statisticRepository.GetTopDriverRaceEntriesInARaceByRaceIdAsync(raceDto.Id, topN: 5);
        
        var top2Constructors = await statisticRepository.GetTopConstructorRaceEntriesInARaceByRaceIdAsync(raceDto.Id, topN: 2);

        return StatisticDtoMapper.MapToTeamOfTheRaceDto(raceDto.Id, raceDto.RaceName, raceDto.Round, top5Drivers, top2Constructors);
    }
}