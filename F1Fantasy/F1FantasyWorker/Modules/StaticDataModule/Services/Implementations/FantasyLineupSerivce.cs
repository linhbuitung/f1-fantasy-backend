using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class FantasyLineupSerivce(IDataSyncRepository dataSyncRepository, WooF1Context context)
    : IFantasyLineupService
{
    // This does not have transaction since its used in the scope of another transaction in Race Service
    public async Task<FantasyLineupDto> AddFantasyLineupAsyncWithNoTransaction(FantasyLineupDto fantasyLineupDto)
    {
        try
        {
            // Check if user and race exist
            AspNetUser user = await dataSyncRepository.GetUserByIdAsync(fantasyLineupDto.UserId);
            Race race = await dataSyncRepository.GetRaceByIdAsync(fantasyLineupDto.RaceId);

            if(user == null)
            {
                throw new Exception($"User with id {fantasyLineupDto.UserId} not found");
            }

            if (race == null)
            {
                throw new Exception($"Race with id {fantasyLineupDto.RaceId} not found");
            }

            FantasyLineup existingFantasyLineup = await dataSyncRepository.GetFantasyLineupByUserIdAndRaceId(fantasyLineupDto.UserId, fantasyLineupDto.RaceId);
            if (existingFantasyLineup != null)
            {
                return null;
            }

            FantasyLineup fantasyLineup = StaticDataDtoMapper.MapDtoToFantasyLineup(fantasyLineupDto);

            FantasyLineup newFantasyLineup = await dataSyncRepository.AddFantasyLineupAsync(fantasyLineup);

            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();
            
            return StaticDataDtoMapper.MapFantasyLineupToDto(newFantasyLineup);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating fantasy lineup: {ex.Message}");

            throw;
        }
    }

    public async Task AddFantasyLineupForAllUsersInASeasonAsync(int year)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var userIds = await dataSyncRepository.GetAllUserIdsAsync();
            var racesInSeason = await dataSyncRepository.GetAllRaceIdsByYearAsync(year);
            var fantasyLineupsInSeason = await dataSyncRepository.GetAllFantasyLineupsInSeasonYearAsync(year);
            
            List<FantasyLineup> newFantasyLineups = new List<FantasyLineup>();
            foreach (var userId in userIds)
            {
                foreach (var raceId in racesInSeason)
                {
                    if (fantasyLineupsInSeason.Exists(fl => fl.UserId == userId && fl.RaceId == raceId))
                    {
                        continue;
                    }
                    
                    FantasyLineupDto fantasyLineupDto = new FantasyLineupDto
                    (
                        userId,
                        raceId
                    );
                    
                    FantasyLineup fantasyLineup = StaticDataDtoMapper.MapDtoToFantasyLineup(fantasyLineupDto);

                    newFantasyLineups.Add(fantasyLineup);
                }
            }
            
            await dataSyncRepository.AddListFantasyLineupsAsync(newFantasyLineups);
            
            await context.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error adding fantasy lineups: {ex.Message}");

            throw;
        }
    }
    
    public async Task AddFantasyLineupForUserInSeasonAsync(int userId, int year)
    {
        var racesInSeason = await dataSyncRepository.GetAllRaceIdsByYearAsync(year);
        foreach (var raceId in racesInSeason)
        {
            FantasyLineup? existing = await dataSyncRepository.GetFantasyLineupByUserIdAndRaceId(userId, raceId);
            if (existing != null) continue;
            FantasyLineupDto dto = new(userId, raceId);
            FantasyLineup lineup = StaticDataDtoMapper.MapDtoToFantasyLineup(dto);
            await dataSyncRepository.AddFantasyLineupAsync(lineup);
        }
        await context.SaveChangesAsync();
    }
}