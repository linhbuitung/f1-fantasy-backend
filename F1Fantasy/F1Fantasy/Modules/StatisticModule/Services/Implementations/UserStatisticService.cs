using F1Fantasy.Modules.StatisticModule.Dtos.Mapper;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;

namespace F1Fantasy.Modules.StatisticModule.Services.Implementations;

public class UserStatisticService(IStatisticRepository statisticRepository, IUserRepository userRepository) : IUserStatisticService
{
    public async Task<List<Dtos.Get.UserWithTotalFantasyPointScoredGetDto>>
        GetTopUsersInSeasonByTotalFantasyPointsAsync(int seasonId)
    {
        var topUsers = await statisticRepository.GetTotalPointsForTopUsersBySeasonAsync(seasonId, topN: 20);
        var result = new List<Dtos.Get.UserWithTotalFantasyPointScoredGetDto>();
        
        foreach (var topUser in topUsers)
        {
            var user = await userRepository.GetUserByIdAsync(topUser.UserId);
            if (user == null)
            {
                continue;
            }

            if (topUser.TotalPoints == null)
            {
                throw new Exception("Total points should not be null here.");
            }
            
            result.Add(StatisticDtoMapper.MapToUserWithTotalFantasyPointScoredGetDto(user, (int)topUser.TotalPoints));
        }
        
        return result
            .OrderByDescending(d => d.TotalFantasyPointScored)
            .ToList();
    }

    public async Task<List<Dtos.Get.UserWithAveragePointScoredGetDto>>
        GetTopUsersInSeasonByAverageFantasyPointsAsync(int seasonId)
    {
        var topUsers = await statisticRepository.GetAveragePointsForTopUsersBySeasonAsync(seasonId, topN: 20);
        var result = new List<Dtos.Get.UserWithAveragePointScoredGetDto>();
        
        foreach (var topUser in topUsers)
        {
            var user = await userRepository.GetUserByIdAsync(topUser.UserId);
            if (user == null)
            {
                continue;
            }

            if (topUser.AveragePoints == null)
            {
                throw new Exception("Total points should not be null here.");
            }
            
            result.Add(StatisticDtoMapper.MapToUserWithAveragePointScoredGetDto(user, (int)topUser.AveragePoints));
        }
        
        return result
            .OrderByDescending(d => d.AverageFantasyPointScored)
            .ToList();
    }

}