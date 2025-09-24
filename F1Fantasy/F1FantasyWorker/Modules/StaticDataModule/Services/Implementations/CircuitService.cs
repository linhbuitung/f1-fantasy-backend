using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations
{
    public class CircuitService(IDataSyncRepository dataSyncRepository, WooF1Context context)
        : ICircuitService
    {
        public async Task<CircuitDto> AddCircuitAsync(CircuitDto circuitDto)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                Circuit existingCircuit = await dataSyncRepository.GetCircuitByCodeAsync(circuitDto.Code);
                if (existingCircuit != null)
                {
                    return null;
                }
                
                // Circuit API returns country name, so we need check for short name.
                Country country = await dataSyncRepository.GetCountryByShortNameAsync(circuitDto.CountryId);
                if (country == null)
                {
                    throw new Exception($"Country with name {circuitDto.CountryId} not found");
                }
                circuitDto.CountryId = country.Id;
                
                Circuit circuit = StaticDataDtoMapper.MapDtoToCircuit(circuitDto);

                Circuit newDircuit = await dataSyncRepository.AddCircuitAsync(circuit);

                // Additional operations that need atomicity (example: logging the event)
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StaticDataDtoMapper.MapCircuitToDto(newDircuit);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating circuit: {ex.Message}");

                throw;
            }
        }

        public async Task AddListCircuitsAsync(List<CircuitDto> circuitDtos)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var existingCountries = await dataSyncRepository.GetAllCountriesAsync();
                var existingCircuitCodes = await dataSyncRepository.GetAllCircuitCodesAsync();
                var newCircuits = new List<Circuit>();
                foreach (var circuitDto in circuitDtos)
                {
                    if (existingCircuitCodes.Contains(circuitDto.Code))
                    {
                        continue; // Skip existing circuits
                    }

                    // Circuit API returns country name, so we need check for short name.
                    if (!existingCountries.Exists(c => c.ShortName.Contains(circuitDto.CountryId)))
                    {
                        throw new Exception($"Country with name {circuitDto.CountryId} not found");
                    }
                    circuitDto.CountryId = existingCountries.First(c => c.ShortName.Contains(circuitDto.CountryId)).Id;

                    Circuit circuit = StaticDataDtoMapper.MapDtoToCircuit(circuitDto);
                    newCircuits.Add(circuit);
                }
                
                var newCircuitsReturned = await dataSyncRepository.AddListCircuitsAsync(newCircuits);
                // Additional operations that need atomicity (example: logging the event)
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating circuit: {ex.Message}");

                throw;
            }
        }

        //get
        public async Task<CircuitDto> GetCircuitByIdAsync(int id)
        {
            Circuit circuit = await dataSyncRepository.GetCircuitByIdAsync(id);
            if (circuit == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapCircuitToDto(circuit);
        }

        public async Task<CircuitDto> GetCircuitByCodeAsync(string code)
        {
            Circuit circuit = await dataSyncRepository.GetCircuitByCodeAsync(code);
            if (circuit == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapCircuitToDto(circuit);
        }

        public async Task<int> GetCircuitsCountAsync()
        {
            return await dataSyncRepository.GetCircuitsCountAsync();
        }
    }
}