using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations;

public class RaceService(IStaticDataRepository staticDataRepository, WooF1Context context)
    : IRaceService
{
    public async Task<RaceDto> AddRaceAsync(RaceDto raceDto)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            Race? existingRace = await staticDataRepository.GetRaceByRaceDateAsync(raceDto.RaceDate);
            if (existingRace != null)
            {
                return null;
            }
            
            // Race API returns circuit, so we need check for circuit.
            Circuit? circuit = await staticDataRepository.GetCircuitByCodeAsync(raceDto.CircuitCode);
            if (circuit == null)
            {
                throw new NotFoundException($"Circuit with code {raceDto.CircuitCode} not found");
            }
            raceDto.CircuitId = circuit.Id;
            
            // Race API returns season, so we need check for season.
            Season? season = await staticDataRepository.GetSeasonByYearAsync(raceDto.RaceDate.Year);
            if (season == null)
            {
                throw new NotFoundException($"Season with year {raceDto.RaceDate.Year} not found");
            }
            raceDto.SeasonId = season.Id;

            Race race = StaticDataDtoMapper.MapDtoToRace(raceDto);

            Race newRace = await staticDataRepository.AddRaceAsync(race);

            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return StaticDataDtoMapper.MapRaceToDto(newRace);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating race: {ex.Message}");

            throw;
        }
    }
    
    public async void AddListRacesAsync(List<RaceDto> raceDtos)
    {
        foreach (var race in raceDtos)
        {
            Console.WriteLine($"Adding race: {race.RaceDate} with circuit code: {race.CircuitCode}");
            await AddRaceAsync(race);
        }
    }
    
    public async Task<RaceDto> GetRaceByIdAsync(int id)
    {
        Race? race = await staticDataRepository.GetRaceByIdAsync(id);
        if (race == null)
        {
            throw new NotFoundException($"Race with id {id} not found");
        }
        return StaticDataDtoMapper.MapRaceToDto(race);
    }

    public async Task<RaceDto> GetRaceByRaceDateAsync(DateOnly date)
    {
        Race? race = await staticDataRepository.GetRaceByRaceDateAsync(date);
        if (race == null)
        {
            throw new NotFoundException($"Race with date {date} not found");
        }
        return StaticDataDtoMapper.MapRaceToDto(race);
    }
}