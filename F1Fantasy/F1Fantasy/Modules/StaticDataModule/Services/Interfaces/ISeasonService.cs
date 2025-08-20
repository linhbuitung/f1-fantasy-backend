using F1Fantasy.Modules.StaticDataModule.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

public interface ISeasonService
{
    Task<SeasonDto> AddSeasonAsync(SeasonDto seasonDto);

    void AddListSeasonsAsync(List<SeasonDto> seasonDtos);

    Task<SeasonDto> GetSeasonByIdAsync(int id);

    Task<SeasonDto> GetSeasonByYearAsync(int year);
}