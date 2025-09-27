using F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

namespace F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;

public interface ICoreGameplayService
{
    Task<Dtos.Get.FantasyLineupDto> GetFantasyLineupByIdAsync(int fantasyLineupId);
    Task<Dtos.Get.FantasyLineupDto> GetFantasyLineupByUserIdAndRaceIdAsync(int userId, int raceId);
    Task<Dtos.Get.FantasyLineupDto> GetCurrentFantasyLineupByUserIdAsync(int userId);
    Task<Dtos.Get.FantasyLineupDto> UpdateFantasyLineupAsync(Dtos.Update.FantasyLineupDto fantasyLineupDto);
    Task ResetFantasyLineupsBySeasonYearAsync(int year);
    
    Task<RaceDto> GetLatestFinishedRaceAsync();
    
    Task<RaceResultDto> GetLatestFinishedRaceResultAsync();
    
    Task<RaceDto> GetLatestRaceAsync();
    Task<RaceDto> GetCurrentRaceAsync();
    
    Task<List<PowerupDto>> GetUsedPowerupsBeforeCurrentRaceByUserInASeasonAsync(int userId);
}