using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.LeagueModule.Dtos.Mapper;
using F1Fantasy.Modules.LeagueModule.Repositories.Interfaces;
using F1Fantasy.Modules.LeagueModule.Services.Interfaces;
using F1Fantasy.Modules.UserModule.Services.Interfaces;

namespace F1Fantasy.Modules.LeagueModule.Services.Implementations;

public class LeagueService(
    IConfiguration configuration,
    ILeagueRepository leagueRepository,
    IUserService userService,
    WooF1Context context)
    : ILeagueService
{
    private int _maxLeaguePerOwner = int.Parse(configuration["CoreGameplaySettings:LeagueSettings:MaxLeaguePerOwner"]);
    private int _maxLeaguePerPlayer = int.Parse(configuration["CoreGameplaySettings:LeagueSettings:MaxLeaguePerPlayer"]);


    public async Task<Dtos.Get.LeagueDto> AddLeagueAsync(Dtos.Create.LeagueDto leagueCreateDto, LeagueType leagueType)
    {

        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Check if owner exists
            _ = userService.GetUserByIdAsync(leagueCreateDto.OwnerId);
        
            await VerifyOwnedLeagueLimitAsync(ownerId: leagueCreateDto.OwnerId);
            
            var league = LeagueDtoMapper.MapCreateDtoToLeague(leagueCreateDto, leagueType);
            var newLeague = await leagueRepository.AddLeagueAsync(league);
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return LeagueDtoMapper.MapLeagueToDto(newLeague, 1, 10);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating league: {ex.Message}");

            throw;
        }

    }

    public async Task<Dtos.Get.LeagueDto> GetLeagueByIdAsync(int leagueId, int pageNum = 1, int pageSize = 10)
    {
        var league = await leagueRepository.GetLeagueByIdIncludesOwnerAndPlayersAsync(leagueId);

        if (league == null)
        {
            throw new NotFoundException("League not found.");
        }
        
        var leagueDto = LeagueDtoMapper.MapLeagueToDto(league, pageNum, pageSize);
        return leagueDto;
    }

    public async Task DeleteLeagueByIdAsync(int leagueId)
    {
        await leagueRepository.DeleteLeagueByIdAsync(leagueId);
    }

    public async Task JoinLeagueAsync(int leagueId, int playerId)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var league = await GetLeagueByIdAsync(leagueId);
            // Check if player exists
            _ = userService.GetUserByIdAsync(playerId);

            var ownedLeagues = await leagueRepository.GetAllLeaguesByOwnerIdAsync(playerId);
            if (ownedLeagues.Select(l => l.Id).Contains(leagueId))
            {
                throw new InvalidOperationException("Owner cannot join their own league.");
            }
            
            await VerifyJoinedLeagueLimitAsync(playerId);
            
            if (league.Type == LeagueType.Private)
            {
                UserLeague userLeague = new UserLeague
                {
                    LeagueId = leagueId,
                    UserId = playerId,
                    IsAccepted = false
                };
                await leagueRepository.AddUserLeagueAsync(userLeague);
            } 
            else if (league.Type == LeagueType.Public)
            {
                UserLeague userLeague = new UserLeague
                {
                    LeagueId = leagueId,
                    UserId = playerId,
                    IsAccepted = true
                };
                await leagueRepository.AddUserLeagueAsync(userLeague);
            }
            else
            {
                throw new InvalidOperationException("Unknown league type.");
            }
            
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error joining league: {ex.Message}");
            throw;
        }
    }
    public async Task<List<Dtos.Get.UserLeagueDto>> GetAllWaitingJoinRequestsAsync(int leagueId)
    {
        var userLeagues = await leagueRepository.GetAllWaitingJoinRequestsByLeagueIdAsync(leagueId);
        return userLeagues.Select(LeagueDtoMapper.MapUserLeagueToDto).ToList();
    }
    public async Task<Dtos.Get.UserLeagueDto> HandleJoinRequestAsync(Dtos.Update.UserLeagueDto userLeagueDto)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var league = await GetLeagueByIdAsync(userLeagueDto.LeagueId);
            // Check if player exists
            _ = userService.GetUserByIdAsync(userLeagueDto.UserId);
            
            await VerifyJoinedLeagueLimitAsync(userLeagueDto.UserId);
            
             if (league.Type == LeagueType.Public)
             {
                 throw new InvalidOperationException("Public leagues do not have join requests.");
             }
            var userLeagueReturnDto = await GetUserLeagueByIdAsync(userLeagueDto.LeagueId, userLeagueDto.UserId);
            
            // Do nothing if the request is already accepted
            if (userLeagueReturnDto.IsAccepted)
            {
                return userLeagueReturnDto;
            }
            
            var updatedUserLeagueReturnDto = await leagueRepository.UpdateUserLeagueAsync(LeagueDtoMapper.MapUpdateDtoToUserLeague(userLeagueDto));
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return LeagueDtoMapper.MapUserLeagueToDto(updatedUserLeagueReturnDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error modifying join request: {ex.Message}");
            throw;
        }
    }

    public async Task<Dtos.Get.UserLeagueDto> GetUserLeagueByIdAsync(int leagueId, int playerId)
    {
        var userLeague = await leagueRepository.GetUserLeagueByIdAsync(leagueId, playerId);
        if (userLeague == null)
        {
            throw new NotFoundException("UserLeague not found.");
        }
        return LeagueDtoMapper.MapUserLeagueToDto(userLeague);
    }

    public async Task LeaveLeagueAsync(int leagueId, int playerId)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var userLeagueDto = await GetUserLeagueByIdAsync(leagueId, playerId);
            
            var league = await GetLeagueByIdAsync(leagueId);
            if (league.Owner.Id == playerId)
            {
                throw new InvalidOperationException("You cannot leave your own league, you can only delete it.");
            }
            await leagueRepository.DeleteUserLeagueByIdAsync(leagueId, playerId);
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error leaving league: {ex.Message}");
            throw;
        }
    }

    private async Task<List<League>> VerifyOwnedLeagueLimitAsync(int ownerId)
    {
        var ownedLeagues = await leagueRepository.GetAllLeaguesByOwnerIdAsync(ownerId);
        if (ownedLeagues.Count >= _maxLeaguePerOwner)
        {
            throw new InvalidOperationException($"Owner with id {ownerId} has reached the maximum number of leagues ({_maxLeaguePerOwner}).");
        }
        return ownedLeagues;
    }
    
    private async Task<List<League>> VerifyJoinedLeagueLimitAsync(int playerId)
    {
        var joinedLeagues = await leagueRepository.GetAllLeaguesByJoinedPlayerIdAsync(playerId);
        if (joinedLeagues.Count >= _maxLeaguePerPlayer)
        {
            throw new InvalidOperationException($"Player with id {playerId} has reached the maximum number of joined leagues ({_maxLeaguePerPlayer}).");
        }
        return joinedLeagues;
    }
}