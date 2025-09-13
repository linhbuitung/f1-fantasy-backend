using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class RaceEntryService : IRaceEntryService
{
    private readonly IStaticDataRepository _staticDataRepository;
    private readonly WooF1Context _context;

    public RaceEntryService(IStaticDataRepository staticDataRepository, WooF1Context context)
    {
        _staticDataRepository = staticDataRepository;
        _context = context;
    }
    
    public async Task<RaceEntryDto> AddRaceEntryAsync(RaceEntryDto raceEntryDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Check if driver and constructor exist
            Driver driver = await _staticDataRepository.GetDriverByCodeAsync(raceEntryDto.DriverCode);
            Constructor constructor = await _staticDataRepository.GetConstructorByCodeAsync(raceEntryDto.ConstructorCode);
            Race race = await _staticDataRepository.GetRaceByRaceDateAsync(raceEntryDto.RaceDate.Value);
            
            if(driver == null)
            {
                throw new Exception($"Driver with code {raceEntryDto.DriverCode} not found");
            }

            if (constructor == null)
            {
                throw new Exception($"Constructor with code {raceEntryDto.ConstructorCode} not found");
            }

            if (race == null)
            {
                throw new Exception($"Race with date {raceEntryDto.RaceDate} not found");
            }
            
            RaceEntry existingRaceEntry = await _staticDataRepository.GetRaceEntryByDriverIdAndRaceDate(driver.Id!, raceEntryDto.RaceDate.Value);
            if (existingRaceEntry != null)
            {
                return null;
            }
            
            RaceEntry raceEntry = StaticDataDtoMapper.MapDtoToRaceEntry(raceEntryDto);
            raceEntry.DriverId = driver.Id;
            raceEntry.ConstructorId = constructor.Id;
            raceEntry.RaceId = race.Id;
    
            RaceEntry newRaceEntry = await _staticDataRepository.AddRaceEntryAsync(raceEntry);
    
            // Additional operations that need atomicity (example: logging the event)
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
    
            return StaticDataDtoMapper.MapRaceEntryToDto(newRaceEntry);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating driver: {ex.Message}");

            throw;
        }
    }

    public async void AddListRaceEntriesAsync(List<RaceEntryDto> raceEntryDtos)
    {
        foreach (var raceEntry in raceEntryDtos)
        {
            await AddRaceEntryAsync(raceEntry);
        }
    }

    public async Task<RaceEntryDto> GetRaceEntryByIdAsync(int id)
    {
        RaceEntry raceEntry = await _staticDataRepository.GetRaceEntryByIdAsync(id);
        if (raceEntry == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapRaceEntryToDto(raceEntry);
    }

    public async Task<int> GetRaceEntriesCountAsync()
    {
        return await _staticDataRepository.GetRaceEntriesCountAsync();
    }
}