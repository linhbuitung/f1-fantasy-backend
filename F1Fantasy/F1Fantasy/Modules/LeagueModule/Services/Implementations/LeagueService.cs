using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.LeagueModule.Dtos.Get;
using F1Fantasy.Modules.LeagueModule.Dtos.Mapper;
using F1Fantasy.Modules.LeagueModule.Repositories.Interfaces;
using F1Fantasy.Modules.LeagueModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using F1Fantasy.Modules.UserModule.Services.Interfaces;
using F1Fantasy.Shared.Dtos;

namespace F1Fantasy.Modules.LeagueModule.Services.Implementations;

public class LeagueService(
    IConfiguration configuration,
    ILeagueRepository leagueRepository,
    IUserService userService,
    IAdminService adminService,
    IStatisticRepository statisticRepository,
    WooF1Context context)
    : ILeagueService
{
    private int _maxLeaguePerOwner = int.Parse(configuration["CoreGameplaySettings:LeagueSettings:MaxLeaguePerOwner"]);
    private int _maxLeaguePerPlayer = int.Parse(configuration["CoreGameplaySettings:LeagueSettings:MaxLeaguePerPlayer"]);


    public async Task<Dtos.Get.LeagueDto> AddLeagueAsync(Dtos.Create.LeagueDto leagueCreateDto, LeagueType leagueType)
    {

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Check if owner exists
            _ = await userService.GetUserByIdAsync(leagueCreateDto.OwnerId);
        
            await VerifyOwnedLeagueLimitAsync(ownerId: leagueCreateDto.OwnerId);
            
            var existingLeague = await leagueRepository.GetLeagueByNameAsync(leagueCreateDto.Name);
            if (existingLeague != null)
            {
                throw new InvalidOperationException($"League with name {leagueCreateDto.Name} already exists.");
            }
            var league = LeagueDtoMapper.MapCreateDtoToLeague(leagueCreateDto, leagueType);
            var newLeague = await leagueRepository.AddLeagueAsync(league);
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return LeagueDtoMapper.MapLeagueToDto(newLeague);
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
        var currentActiveSeason = await adminService.GetActiveSeasonAsync();
        var league = await leagueRepository.GetLeagueByIdIncludesOwnerAndPlayersAsync(leagueId);
        
        if (league == null)
        {
            throw new NotFoundException("League not found.");
        }
        var usersInLeague = await leagueRepository.GetAllUserLeaguesByLeagueIdAndAcceptStatusAsync(leagueId, isAccepted:true, currentSeasonId: currentActiveSeason.Id);
        var userInLeagueDtos = usersInLeague.Select(ul => LeagueDtoMapper.MapUserLeagueToUserInLeagueDto(ul, currentActiveSeason.Id)).ToList();
        
        var ownerTotalPoints = await statisticRepository.GetTotalPointOfAnUserBySeasonIdAsync(league.OwnerId, currentActiveSeason.Id);
        userInLeagueDtos.Add(LeagueDtoMapper.MapUserWithTotalPointToUserInLeagueDto(league.User, ownerTotalPoints));
        
        var leagueDto = LeagueDtoMapper.MapLeagueToDtoWithPlayers(league, userInLeagueDtos, pageNum, pageSize);
        return leagueDto;
    }
    
    public async Task<List<Dtos.Get.LeagueDto>> GetJoinedLeaguesByUserIdAsync(int userId)
    {
        // Check if user exists
        _ = await userService.GetUserByIdAsync(userId);
        
        var joinedLeagues = await leagueRepository.GetAllLeaguesByJoinedPlayerIdAsync(userId);
        
        return joinedLeagues.Select(LeagueDtoMapper.MapLeagueToDto).ToList();
        
    }
    
    public async Task<List<Dtos.Get.LeagueDto>> GetOwnedLeaguesByUserIdAsync(int userId)
    {
        // Check if user exists
        _ = await userService.GetUserByIdAsync(userId);
        
        var ownedLeagues = await leagueRepository.GetAllLeaguesByOwnerIdAsync(userId);
        
        return ownedLeagues.Select(LeagueDtoMapper.MapLeagueToDto).ToList();
        
    }

    public async Task DeleteLeagueByIdAsync(int leagueId)
    {
        await leagueRepository.DeleteLeagueByIdAsync(leagueId);
    }

    public async Task JoinLeagueAsync(int leagueId, int playerId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var league = await GetLeagueByIdAsync(leagueId);
            // Check if player exists
            _ = await userService.GetUserByIdAsync(playerId);

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
    public async Task<Dtos.Get.UserLeagueDto?> HandleJoinRequestAsync(Dtos.Update.UserLeagueDto userLeagueDto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if(!userLeagueDto.IsAccepted)
            {
                // Delete the join request if not accepted
                await leagueRepository.DeleteUserLeagueByIdAsync(userLeagueDto.LeagueId, userLeagueDto.UserId);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return null;
            }
            var league = await GetLeagueByIdAsync(userLeagueDto.LeagueId);
            // Check if player exists
            _ = await userService.GetUserByIdAsync(userLeagueDto.UserId);
            
            // If the requester has the max amount of leagues joined, delete the request and throw an error
            await VerifyJoinedLeagueLimitWithDeletionAsync(userLeagueDto);
            
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

    public async Task RemovePlayerFromLeagueAsync(int leagueId, int playerId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
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
    
    // Same as VerifyJoinedLeagueLimitAsync but delete the join requests if the limit is reached
    private async Task<List<League>> VerifyJoinedLeagueLimitWithDeletionAsync(Dtos.Update.UserLeagueDto userLeagueDto)
    {
        var joinedLeagues = await leagueRepository.GetAllLeaguesByJoinedPlayerIdAsync(userLeagueDto.UserId);
        if (joinedLeagues.Count >= _maxLeaguePerPlayer && !userLeagueDto.IsAccepted)
        {
            await leagueRepository.DeleteUserLeagueByIdAsync(userLeagueDto.LeagueId, userLeagueDto.UserId);
            await context.SaveChangesAsync();
            throw new InvalidOperationException($"Player with id {userLeagueDto.UserId} has reached the maximum number of joined leagues ({_maxLeaguePerPlayer}). Deleting the join request.");
        }
        return joinedLeagues;
    }

    public async Task<PagedResult<Dtos.Get.LeagueDto>> SearchLeaguesAsync(string query, int pageNum, int pageSize)
    {
        int skip = (pageNum - 1) * pageSize;
        var (leagues, totalCount) = await leagueRepository.SearchLeaguesViaFullTextSearchAsync(query, skip, pageSize);

        var items = leagues.Select(LeagueDtoMapper.MapSearchedLeagueToDto).ToList();

        return new PagedResult<Dtos.Get.LeagueDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNum = pageNum,
            PageSize = pageSize
        };
    }
    
    public async Task<Dtos.Get.LeagueDto> UpdateLeagueAsync(Dtos.Update.LeagueDto leagueUpdateDto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var trackedLeague = await leagueRepository.GetTrackedLeagueByLeagueIdAndOwnerIdAsync(leagueUpdateDto.Id, leagueUpdateDto.OwnerId);
            if (trackedLeague == null)
            {
                throw new NotFoundException("League not found.");
            }
        
            var updatedLeague = LeagueDtoMapper.MapUpdateDtoToLeague(leagueUpdateDto, trackedLeague);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return LeagueDtoMapper.MapLeagueToDto(updatedLeague);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating league: {ex.Message}");

            throw;
        }
    }

    public async Task<List<Dtos.Get.UserLeagueDto>> GetUnAcceptedUserLeagueByLeagueIdAsync(int leagueId)
    {
        var userLeagues = await leagueRepository.GetAllUserLeaguesByLeagueIdAndAcceptStatusAsync(leagueId, false, currentSeasonId: null);
        return userLeagues.Select(LeagueDtoMapper.MapUserLeagueToDto).ToList();
    }
}