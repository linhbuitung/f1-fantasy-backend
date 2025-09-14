using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class FantasyLineupSerivce : IFantasyLineupService
{
    private readonly IDataSyncRepository _dataSyncRepository;
    private readonly WooF1Context _context;

    public FantasyLineupSerivce(IDataSyncRepository dataSyncRepository, WooF1Context context)
    {
        _dataSyncRepository = dataSyncRepository;
        _context = context;
    }

    // This does not have transaction since its used in the scope of another transaction in Race Service
    public async Task<FantasyLineupDto> AddFantasyLineupAsyncWithNoTransaction(FantasyLineupDto fantasyLineupDto)
    {
        try
        {
            // Check if user and race exist
            AspNetUser user = await _dataSyncRepository.GetUserByIdAsync(fantasyLineupDto.UserId);
            Race race = await _dataSyncRepository.GetRaceByIdAsync(fantasyLineupDto.RaceId);

            if(user == null)
            {
                throw new Exception($"User with id {fantasyLineupDto.UserId} not found");
            }

            if (race == null)
            {
                throw new Exception($"Race with id {fantasyLineupDto.RaceId} not found");
            }

            FantasyLineup existingFantasyLineup = await _dataSyncRepository.GetFantasyLineupByUserIdAndRaceId(fantasyLineupDto.UserId, fantasyLineupDto.RaceId);
            if (existingFantasyLineup != null)
            {
                return null;
            }

            FantasyLineup fantasyLineup = StaticDataDtoMapper.MapDtoToFantasyLineup(fantasyLineupDto);

            FantasyLineup newFantasyLineup = await _dataSyncRepository.AddFantasyLineupAsync(fantasyLineup);

            // Additional operations that need atomicity (example: logging the event)
            await _context.SaveChangesAsync();
            
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
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var userIds = await _dataSyncRepository.GetAllUserIdsAsync();
            var racesInSeason = await _dataSyncRepository.GetAllRaceIdsByYearAsync(year);

            foreach (var userId in userIds)
            {
                foreach (var raceId in racesInSeason)
                {
                    FantasyLineup existingFantasyLineup = await _dataSyncRepository.GetFantasyLineupByUserIdAndRaceId(userId, raceId);
                    if (existingFantasyLineup != null)
                    {
                        continue;
                    }
                    
                    FantasyLineupDto fantasyLineupDto = new FantasyLineupDto
                    (
                        userId,
                        raceId
                    );
                    
                    FantasyLineup fantasyLineup = StaticDataDtoMapper.MapDtoToFantasyLineup(fantasyLineupDto);

                    FantasyLineup newFantasyLineup = await _dataSyncRepository.AddFantasyLineupAsync(fantasyLineup);
                }
            }
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error adding fantasy lineups: {ex.Message}");

            throw;
        }

    }
}