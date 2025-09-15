using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.LeagueModule.Dtos.Mapper;
using F1Fantasy.Modules.LeagueModule.Repositories.Interfaces;
using F1Fantasy.Modules.LeagueModule.Services.Interfaces;
using F1Fantasy.Modules.UserModule.Services.Interfaces;

namespace F1Fantasy.Modules.LeagueModule.Services.Implementations;

public class LeagueService : ILeagueService
{
    /*  "CoreGameplaySettings": {
    "LeagueSettings": {
      "MaxAllowedPlayer": 100,
      "MaxLeaguePerOwner": 3,
      "MaxLeaguePerPlayer": 5
    }
  }*/
    private readonly IConfiguration _configuration;
    private readonly ILeagueRepository _leagueRepository;
    private readonly IUserService _userService;
    private readonly WooF1Context _context;

    public LeagueService(IConfiguration configuration, ILeagueRepository leagueRepository, IUserService userService, WooF1Context context)
    {
        _configuration = configuration;
        _leagueRepository = leagueRepository;
        _userService = userService;
        _context = context;
    }

    
    public async Task<Dtos.Get.LeagueDto> AddLeagueAsync(Dtos.Create.LeagueDto leagueCreateDto, LeagueType leagueType)
    {
        var maxLeaguePerOwner = int.Parse(_configuration["CoreGameplaySettings:LeagueSettings:MaxLeaguePerOwner"]);
        var maxLeaguePerPlayer = int.Parse(_configuration["CoreGameplaySettings:LeagueSettings:MaxLeaguePerPlayer"]);
        
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Check if owner exists
            _ = _userService.GetUserByIdAsync(leagueCreateDto.OwnerId);
        
            var ownedLeagues = await _leagueRepository.GetAllLeaguesByOwnerIdAsync(leagueCreateDto.OwnerId);
            if (ownedLeagues.Count >= maxLeaguePerOwner)
            {
                throw new InvalidOperationException($"Owner with id {leagueCreateDto.OwnerId} has reached the maximum number of leagues ({maxLeaguePerOwner}).");
            }
        
            var joinedLeagues = await _leagueRepository.GetAllLeaguesByJoinedPlayerIdAsync(leagueCreateDto.OwnerId);
            if (joinedLeagues.Count >= maxLeaguePerPlayer)
            {
                throw new InvalidOperationException($"Player with id {leagueCreateDto.OwnerId} has reached the maximum number of joined leagues ({maxLeaguePerPlayer}).");
            }
            
            var league = LeagueDtoMapper.MapCreateDtoToLeague(leagueCreateDto, leagueType);
            var newLeague = await _leagueRepository.AddLeagueAsync(league);
            
            await _context.SaveChangesAsync();
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
        var league = await _leagueRepository.GetLeagueByIdIncludesOwnerAndPlayersAsync(leagueId);

        if (league == null)
        {
            throw new NotFoundException("League not found.");
        }
        
        var leagueDto = LeagueDtoMapper.MapLeagueToDto(league, pageNum, pageSize);
        return leagueDto;
    }

    public async Task DeleteLeagueByIdAsync(int leagueId)
    {
        await _leagueRepository.DeleteLeagueByIdAsync(leagueId);
    }

    public async Task JoinLeagueAsync(int leagueId, int playerId)
    {
        var league = await GetLeagueByIdAsync(leagueId);
        // Check if player exists
        _ = _userService.GetUserByIdAsync(playerId);

        if (league.Type == LeagueType.Private)
        {
            UserLeague userLeague = new UserLeague
            {
                LeagueId = leagueId,
                UserId = playerId,
               IsAccepted = false
            };
            await _leagueRepository.AddUserLeagueAsync(userLeague);
        } 
        else if (league.Type == LeagueType.Public)
        {
            UserLeague userLeague = new UserLeague
            {
                LeagueId = leagueId,
                UserId = playerId,
                IsAccepted = true
            };
            await _leagueRepository.AddUserLeagueAsync(userLeague);
        }
        else
        {
            throw new InvalidOperationException("Unknown league type.");
        }
    }
}