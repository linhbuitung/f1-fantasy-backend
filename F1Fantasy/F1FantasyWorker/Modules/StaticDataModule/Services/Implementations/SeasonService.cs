using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class SeasonService : ISeasonService
{
    private readonly IStaticDataRepository _staticDataRepository;
    private readonly WooF1Context _context;

    public SeasonService(IStaticDataRepository staticDataRepository, WooF1Context context)
    {
        _staticDataRepository = staticDataRepository;
        _context = context;
    }
    
    public async Task<SeasonDto> AddSeasonAsync(SeasonDto seasonDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            Season existingSeason = await _staticDataRepository.GetSeasonByYearAsync(seasonDto.Year);
            if (existingSeason != null)
            {
                return null;
            }

            Season season = StaticDataDtoMapper.MapDtoToSeason(seasonDto);

            Season newSeason = await _staticDataRepository.AddSeasonAsync(season);

            // Additional operations that need atomicity (example: logging the event)
            await _context.SaveChangesAsync();

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
        Season season = await _staticDataRepository.GetSeasonByIdAsync(id);
        if (season == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapSeasonToDto(season);
    }

    public async Task<SeasonDto> GetSeasonByYearAsync(int year)
    {
        Season season = await _staticDataRepository.GetSeasonByYearAsync(year);
        if (season == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapSeasonToDto(season);
    }
}