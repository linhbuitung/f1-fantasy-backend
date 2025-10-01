using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations;

public class SeasonService(IStaticDataRepository staticDataRepository, WooF1Context context)
    : ISeasonService
{
    public async Task<SeasonDto> AddSeasonAsync(SeasonDto seasonDto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            Season? existingSeason = await staticDataRepository.GetSeasonByYearAsync(seasonDto.Year);
            if (existingSeason != null)
            {
                return null;
            }

            Season season = StaticDataDtoMapper.MapDtoToSeason(seasonDto);

            Season newSeason = await staticDataRepository.AddSeasonAsync(season);

            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return StaticDataDtoMapper.MapSeasonToDto(newSeason);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating season: {ex.Message}");

            throw;
        }
    }

    public async void AddListSeasonsAsync(List<SeasonDto> seasonDtos)
    {
        foreach (var season in seasonDtos)
        {
            Console.WriteLine($"Adding season: {season.Year}");
            await AddSeasonAsync(season);
        }
    }

    public async Task<SeasonDto> GetSeasonByIdAsync(int id)
    {
        Season? season = await staticDataRepository.GetSeasonByIdAsync(id);
        if (season == null)
        {
            throw new NotFoundException($"Season with id {id} not found");
        }
        return StaticDataDtoMapper.MapSeasonToDto(season);
    }

    public async Task<SeasonDto> GetSeasonByYearAsync(int year)
    {
        Season? season = await staticDataRepository.GetSeasonByYearAsync(year);
        if (season == null)
        {
            throw new NotFoundException($"Season with year {year} not found");
        }
        return StaticDataDtoMapper.MapSeasonToDto(season);
    }
}