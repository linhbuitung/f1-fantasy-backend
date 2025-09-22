using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.AuthModule.Repositories.Interfaces;

public interface IAuthExtensionRepository
{
    Task<List<int>> GetAllRaceIdsByYearAsync(int year);
    Task<FantasyLineup?> GetFantasyLineupByUserIdAndRaceId(int userId, int raceId);

    Task<FantasyLineup> AddFantasyLineupAsync(FantasyLineup fantasyLineup);
}