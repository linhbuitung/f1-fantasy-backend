using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

public interface ISeasonService
{
    Task<SeasonDto> AddSeasonAsync(SeasonDto seasonDto);

    Task AddListSeasonsAsync(List<SeasonDto> seasonDtos);

    Task<SeasonDto> GetSeasonByIdAsync(int id);

    Task<SeasonDto> GetSeasonByYearAsync(int year);

    Task<int> GetSeasonsCountAsync();
}
