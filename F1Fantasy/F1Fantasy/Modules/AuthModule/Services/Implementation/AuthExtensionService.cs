using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AuthModule.Dtos.Create;
using F1Fantasy.Modules.AuthModule.Dtos.Mapper;
using F1Fantasy.Modules.AuthModule.Repositories.Interfaces;
using F1Fantasy.Modules.AuthModule.Services.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;

namespace F1Fantasy.Modules.AuthModule.Services.Implementation;

public class AuthExtensionService(WooF1Context context, IAuthExtensionRepository authExtensionRepository) : IAuthExtensionService
{
    public async Task AddFantasyLineupForUserInSeasonAsync(int userId, int year)
    {
        
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var racesInSeason = await authExtensionRepository.GetAllRaceIdsByYearAsync(year);
            foreach (var raceId in racesInSeason)
            {
                FantasyLineup? existing = await authExtensionRepository.GetFantasyLineupByUserIdAndRaceId(userId, raceId);
                if (existing != null) continue;
                FantasyLineupDto dto = new FantasyLineupDto
                {
                    UserId = userId,
                    RaceId = raceId
                };
                FantasyLineup lineup = AuthExtensionMapper.MapCreateDtoToFantasyLineup(dto);
                await authExtensionRepository.AddFantasyLineupAsync(lineup);
            }
            await context.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error adding fantasy lineup: {ex.Message}");
            throw;
        }
    }

    public async Task SetUserJoinDateAsync(ApplicationUser user, DateOnly date)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            user.JoinDate = date;
            await context.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error setting join date: {ex.Message}");
            throw;
        }
    }

}