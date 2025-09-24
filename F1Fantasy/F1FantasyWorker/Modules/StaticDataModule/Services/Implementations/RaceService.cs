using AutoMapper;
using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class RaceService(
    IDataSyncRepository dataSyncRepository,
    IFantasyLineupService fantasyLineupService,
    WooF1Context context)
    : IRaceService
{
    private readonly IFantasyLineupService _fantasyLineupService = fantasyLineupService;

    // Add new race and  add new fantasy lineups for all users for the new race
    public async Task<RaceDto> AddRaceAsync(RaceDto raceDto)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var existingRace = await dataSyncRepository.GetRaceByRaceDateAsync(raceDto.RaceDate);
            if (existingRace != null)
            {
                return null;
            }
            
            // Race API returns circuit, so we need check for circuit.
            Circuit circuit = await dataSyncRepository.GetCircuitByCodeAsync(raceDto.CircuitCode);
            if (circuit == null)
            {
                throw new Exception($"Circuit with code {raceDto.CircuitCode} not found");
            }
            raceDto.CircuitId = circuit.Id;
            
            // Race API returns season, so we need check for season.
            var season = await dataSyncRepository.GetSeasonByYearAsync(raceDto.RaceDate.Year);
            if (season == null)
            {
                throw new Exception($"Season with year {raceDto.RaceDate.Year} not found");
            }
            raceDto.SeasonId = season.Id;

            Race race = StaticDataDtoMapper.MapDtoToRace(raceDto);

            Race newRace = await dataSyncRepository.AddRaceAsync(race);
            
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
    
    public async Task AddListRacesAsync(List<RaceDto> raceDtos)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var existingCircuits = await dataSyncRepository.GetAllCircuitsAsync();
            var existingSeasons = await dataSyncRepository.GetAllSeasonsAsync();
            var existingRaceDates = await dataSyncRepository.GetAllRaceDatesAsync();
            var newRaces = new List<Race>();
            foreach (var raceDto in raceDtos)
            {
                if (existingRaceDates.Contains(raceDto.RaceDate))
                {
                    continue; // Skip existing
                }

                // Race API returns circuit, so we need check for circuit.
                if (!existingCircuits.Exists(c => c.Code == raceDto.CircuitCode))
                {
                    throw new Exception($"Circuit with code {raceDto.CircuitCode} not found");
                }

                raceDto.CircuitId = existingCircuits.First(c => c.Code == raceDto.CircuitCode).Id;

                // Race API returns season, so we need check for season.
                if (!existingSeasons.Exists(s => s.Year == raceDto.RaceDate.Year))
                {
                    throw new Exception($"Season with year {raceDto.RaceDate.Year} not found");
                }

                raceDto.SeasonId = existingSeasons.First(s => s.Year == raceDto.RaceDate.Year).Id;

                Race race = StaticDataDtoMapper.MapDtoToRace(raceDto);
                newRaces.Add(race);
            }

            var newRacesReturned = await dataSyncRepository.AddListRacesAsync(newRaces);

            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating race: {ex.Message}");

            throw;
        }
    }
    
    public async Task<RaceDto> GetRaceByIdAsync(int id)
    {
        var race = await dataSyncRepository.GetRaceByIdAsync(id);
        if (race == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapRaceToDto(race);
    }

    public async Task<RaceDto> GetRaceByRaceDateAsync(DateOnly date)
    {
        var race = await dataSyncRepository.GetRaceByRaceDateAsync(date);
        if (race == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapRaceToDto(race);
    }

    public async Task<int> GetRacesCountAsync()
    {
        return await dataSyncRepository.GetRacesCountAsync();
    }

}