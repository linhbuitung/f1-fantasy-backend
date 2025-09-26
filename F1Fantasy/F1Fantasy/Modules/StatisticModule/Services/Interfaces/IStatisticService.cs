namespace F1Fantasy.Modules.StatisticModule.Services.Interfaces;

public interface IStatisticService
{
    Task<Dtos.Get.GeneralSeasonStatisticDto> GetGeneralStatisticBySeasonId(int seasonId);
}