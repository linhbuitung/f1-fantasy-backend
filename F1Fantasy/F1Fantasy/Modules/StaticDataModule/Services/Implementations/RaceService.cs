using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations;

public class RaceService : IRaceService
{
    private readonly IStaticDataRepository _staticDataRepository;
    private readonly WooF1Context _context;

    public RaceService(IStaticDataRepository staticDataRepository, WooF1Context context)
    {
        _staticDataRepository = staticDataRepository;
        _context = context;
    }
    
    public async Task<RaceDto> AddRaceAsync(RaceDto raceDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            Race existingRace = await _staticDataRepository.GetRaceByRaceDateAsync(raceDto.RaceDate);
            if (existingRace != null)
            {
                return null;
            }
            
            // Race API returns circuit, so we need check for circuit.
            Circuit circuit = await _staticDataRepository.GetCircuitByCodeAsync(raceDto.CircuitCode);
            if (circuit == null)
            {
                throw new Exception($"Circuit with code {raceDto.CircuitCode} not found");
            }
            raceDto.CircuitId = circuit.Id;

            Race race = StaticDataDtoMapper.MapDtoToRace(raceDto);

            Race newRace = await _staticDataRepository.AddRaceAsync(race);

            // Additional operations that need atomicity (example: logging the event)
            await _context.SaveChangesAsync();

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
        Race race = await _staticDataRepository.GetRaceByIdAsync(id);
        if (race == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapRaceToDto(race);
    }

    public async Task<RaceDto> GetRaceByRaceDateAsync(DateOnly date)
    {
        Race race = await _staticDataRepository.GetRaceByRaceDateAsync(date);
        if (race == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapRaceToDto(race);
    }
}