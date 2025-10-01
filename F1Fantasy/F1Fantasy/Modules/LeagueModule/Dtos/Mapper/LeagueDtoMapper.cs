using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.LeagueModule.Dtos.Mapper;

public class LeagueDtoMapper
{
    public static League MapCreateDtoToLeague(Dtos.Create.LeagueDto dto, LeagueType leagueType)
    {
        return new League
        {
            Name = dto.Name,
            Type = leagueType,
            Description = dto.Description,
            MaxPlayersNum = dto.MaxPlayersNum,
            OwnerId = dto.OwnerId
        };
    }

    public static Get.LeagueDto MapLeagueToDto(League league )
    {
        return new Get.LeagueDto
        {
            Id = league.Id,
            Name = league.Name,
            Type = league.Type,
            Description = league.Description,
            MaxPlayersNum = league.MaxPlayersNum
            
        };
    }
    
    public static Get.LeagueDto MapLeagueToDtoWithPlayers(League league, int pageNum, int pageSize )
    {
        return new Get.LeagueDto
        {
            Id = league.Id,
            Name = league.Name,
            Type = league.Type,
            Description = league.Description,
            MaxPlayersNum = league.MaxPlayersNum,
            Owner = MapUserInLeagueToDto(league.User),
            Users = league.UserLeagues
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .Select(ul => MapUserInLeagueToDto(ul.User))
                .ToList()
        };
    }

    public static Get.LeagueDto MapSearchedLeagueToDto(League league)
    {
        return new Get.LeagueDto
        {
            Id = league.Id,
            Name = league.Name,
            Type = league.Type,
            Description = league.Description,
            MaxPlayersNum = league.MaxPlayersNum,
            Owner = MapUserInLeagueToDto(league.User)
        };
    }
    
    private static Get.UserInLeagueDto MapUserInLeagueToDto(ApplicationUser user)
    {
        return new Get.UserInLeagueDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            CountryName = user.Country?.ShortName ?? string.Empty,
        };
    }

    public static Get.UserLeagueDto MapUserLeagueToDto(UserLeague userLeague)
    {
        return new Get.UserLeagueDto
        {
            UserId = userLeague.UserId,
            LeagueId = userLeague.LeagueId,
            UserDisplayName = userLeague.User.DisplayName ?? null,
            UserEmail = userLeague.User.Email ?? string.Empty,
            IsAccepted = userLeague.IsAccepted,
        };
    }
    
    public static UserLeague MapUpdateDtoToUserLeague(Dtos.Update.UserLeagueDto dto)
    {
        return new UserLeague
        {
            UserId = dto.UserId,
            LeagueId = dto.LeagueId,
            IsAccepted = dto.IsAccepted,
        };
    }
    
    public static League MapUpdateDtoToLeague(Dtos.Update.LeagueDto dto, League trackedLeague)
    {
        if (dto.Name != null) trackedLeague.Name = dto.Name;
        if (dto.Description != null) trackedLeague.Description = dto.Description;
        return trackedLeague;
    }
} 