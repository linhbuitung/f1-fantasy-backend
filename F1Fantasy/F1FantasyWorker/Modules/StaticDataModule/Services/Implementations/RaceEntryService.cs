using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class RaceEntryService(IDataSyncRepository dataSyncRepository, WooF1Context context)
    : IRaceEntryService
{
    public async Task<RaceEntryDto> AddRaceEntryAsync(RaceEntryDto raceEntryDto)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Check if driver and constructor exist
            Driver driver = await dataSyncRepository.GetDriverByCodeAsync(raceEntryDto.DriverCode);
            Constructor constructor = await dataSyncRepository.GetConstructorByCodeAsync(raceEntryDto.ConstructorCode);
            Race race = await dataSyncRepository.GetRaceByRaceDateAsync(raceEntryDto.RaceDate.Value);
            
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
            
            RaceEntry existingRaceEntry = await dataSyncRepository.GetRaceEntryByDriverIdAndRaceDate(driver.Id!, raceEntryDto.RaceDate.Value);
            if (existingRaceEntry != null)
            {
                return null;
            }
            
            RaceEntry raceEntry = StaticDataDtoMapper.MapDtoToRaceEntry(raceEntryDto);
            raceEntry.DriverId = driver.Id;
            raceEntry.ConstructorId = constructor.Id;
            raceEntry.RaceId = race.Id;
    
            RaceEntry newRaceEntry = await dataSyncRepository.AddRaceEntryAsync(raceEntry);
    
            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();

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

    public async Task AddListRaceEntriesAsync(List<RaceEntryDto> raceEntryDtos)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var existingDrivers = await dataSyncRepository.GetAllDriversAsync();
            var existingConstructors = await dataSyncRepository.GetAllConstructorsAsync();
            var existingRaces = await dataSyncRepository.GetAllRacesAsync();
            
            var existingRaceEntries = await dataSyncRepository.GetAllRaceEntriesBySeasonYearAsync(DateTime.UtcNow.Year);
            var newRaceEntries = new List<RaceEntry>();
            foreach (var raceEntryDto in raceEntryDtos)
            {
                if(raceEntryDto.DriverCode == null || !existingDrivers.Exists(d => d.Code == raceEntryDto.DriverCode))
                {
                    throw new Exception($"Driver with code {raceEntryDto.DriverCode} not found");
                }


                if (raceEntryDto.ConstructorCode == null || !existingConstructors.Exists(c => c.Code == raceEntryDto.ConstructorCode))
                {
                    throw new Exception($"Constructor with code {raceEntryDto.ConstructorCode} not found");
                }

                if (raceEntryDto.RaceDate == null || !existingRaces.Exists(r => r.RaceDate == raceEntryDto.RaceDate.Value))
                {
                    throw new Exception($"Race with date {raceEntryDto.RaceDate} not found");
                }
                
                // if race entry with userId and race date already exists, skip
                if (existingRaceEntries.Any(re => re.Driver.Code == raceEntryDto.DriverCode && re.Race.RaceDate == raceEntryDto.RaceDate))
                {
                    continue;
                }
                
                RaceEntry raceEntry = StaticDataDtoMapper.MapDtoToRaceEntry(raceEntryDto);
                raceEntry.DriverId = existingDrivers.First(d => d.Code == raceEntryDto.DriverCode).Id;
                raceEntry.ConstructorId = existingConstructors.First(c => c.Code == raceEntryDto.ConstructorCode).Id;
                raceEntry.RaceId = existingRaces.First(r => r.RaceDate == raceEntryDto.RaceDate.Value).Id;
    
                newRaceEntries.Add(raceEntry);
            }
            await dataSyncRepository.AddListRaceEntriesAsync(newRaceEntries);
    
            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating driver: {ex.Message}");

            throw;
        }
    }

    public async Task<RaceEntryDto> GetRaceEntryByIdAsync(int id)
    {
        RaceEntry raceEntry = await dataSyncRepository.GetRaceEntryByIdAsync(id);
        if (raceEntry == null)
        {
            return null;
        }
        return StaticDataDtoMapper.MapRaceEntryToDto(raceEntry);
    }

    public async Task<int> GetRaceEntriesCountAsync()
    {
        return await dataSyncRepository.GetRaceEntriesCountAsync();
    }
}