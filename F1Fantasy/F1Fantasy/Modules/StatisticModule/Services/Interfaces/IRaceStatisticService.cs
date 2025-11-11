using F1Fantasy.Core.Common;
using F1Fantasy.Modules.StatisticModule.Dtos.Get;

namespace F1Fantasy.Modules.StatisticModule.Services.Interfaces;

public interface IRaceStatisticService
{
    Task<RaceStatisticDto> GetRaceStatisticByIdAsync(int raceId);
}