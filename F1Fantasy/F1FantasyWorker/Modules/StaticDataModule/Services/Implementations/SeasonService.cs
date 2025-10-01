using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class SeasonService(IDataSyncRepository dataSyncRepository, WooF1Context context)
    : ISeasonService
{
    public async Task<SeasonDto> AddSeasonAsync(SeasonDto seasonDto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            Season existingSeason = await dataSyncRepository.GetSeasonByYearAsync(seasonDto.Year);
            if (existingSeason != null)
            {
                return null;
            }

            Season season = StaticDataDtoMapper.MapDtoToSeason(seasonDto);

            Season newSeason = await dataSyncRepository.AddSeasonAsync(season);

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

    public async Task AddListSeasonsAsync(List<SeasonDto> seasonDtos)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var existingSeasonYears = await dataSyncRepository.GetAllSeasonYearsAsync();
            var newSeasons = new List<Season>();
            foreach (var seasonDto in seasonDtos)
            {
                if (existingSeasonYears.Contains(seasonDto.Year))
                {
                    continue;
                }
                
                Season season = StaticDataDtoMapper.MapDtoToSeason(seasonDto);
                newSeasons.Add(season);
            }

            var newSeasonsReturned = await dataSyncRepository.AddListSeasonsAsync(newSeasons);

            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating season: {ex.Message}");

            throw;
        }
    }

    public async Task<SeasonDto> GetSeasonByIdAsync(int id)
    {
        Season season = await dataSyncRepository.GetSeasonByIdAsync(id);
        if (season == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapSeasonToDto(season);
    }

    public async Task<SeasonDto> GetSeasonByYearAsync(int year)
    {
        Season season = await dataSyncRepository.GetSeasonByYearAsync(year);
        if (season == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapSeasonToDto(season);
    }
    
    public async Task<int> GetSeasonsCountAsync()
    {
        return await dataSyncRepository.GetSeasonsCountAsync();
    }
}