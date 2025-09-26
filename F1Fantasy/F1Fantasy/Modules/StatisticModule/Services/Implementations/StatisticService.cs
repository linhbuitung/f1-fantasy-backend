using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;

namespace F1Fantasy.Modules.StatisticModule.Services.Implementations;

public class StatisticService(IStatisticRepository statisticRepository) : IStatisticService
{
    public async Task<Dtos.Get.GeneralSeasonStatisticDto> GetGeneralStatisticBySeasonId(int seasonId)
    {
        var highestPoint = await statisticRepository.GetHighestScoreBySeasonIdAsync(seasonId);
        var averagePoints = await statisticRepository.GetAverageScoreBySeasonIdAsync(seasonId);
        var mostPickedDriver = await statisticRepository.GetMostPickedDriverAsync(seasonId);
        var totalTransferMade = await statisticRepository.GetTotalTransfersBySeasonIdAsync(seasonId);
        
        return new Dtos.Get.GeneralSeasonStatisticDto
        {
            HighestPoint = highestPoint,
            AveragePoints = averagePoints,
            MostPickedDriver = mostPickedDriver,
            TotalTransferMade = totalTransferMade
        };
    }

}