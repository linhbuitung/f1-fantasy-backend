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

    public static Get.LeagueDto MapLeagueToDto(League league, int pageNum, int pageSize )
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
} 