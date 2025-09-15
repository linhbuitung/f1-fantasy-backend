using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations
{
    public class CircuitService : ICircuitService
    {
        private readonly IDataSyncRepository _dataSyncRepository;
        private readonly WooF1Context _context;

        public CircuitService(IDataSyncRepository dataSyncRepository, WooF1Context context)
        {
            _dataSyncRepository = dataSyncRepository;
            _context = context;
        }

        public async Task<CircuitDto> AddCircuitAsync(CircuitDto circuitDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Circuit existingCircuit = await _dataSyncRepository.GetCircuitByCodeAsync(circuitDto.Code);
                if (existingCircuit != null)
                {
                    return null;
                }
                
                // Circuit API returns country name, so we need check for short name.
                Country country = await _dataSyncRepository.GetCountryByShortNameAsync(circuitDto.CountryId);
                if (country == null)
                {
                    throw new Exception($"Country with name {circuitDto.CountryId} not found");
                }
                circuitDto.CountryId = country.Id;
                
                Circuit circuit = StaticDataDtoMapper.MapDtoToCircuit(circuitDto);

                Circuit newDircuit = await _dataSyncRepository.AddCircuitAsync(circuit);

                // Additional operations that need atomicity (example: logging the event)
                await _context.SaveChangesAsync();

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

        public async void AddListCircuitsAsync(List<CircuitDto> circuitDtos)
        {
            foreach (var circuit in circuitDtos)
            {
                await AddCircuitAsync(circuit);
            }
        }

        //get
        public async Task<CircuitDto> GetCircuitByIdAsync(int id)
        {
            Circuit circuit = await _dataSyncRepository.GetCircuitByIdAsync(id);
            if (circuit == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapCircuitToDto(circuit);
        }

        public async Task<CircuitDto> GetCircuitByCodeAsync(string code)
        {
            Circuit circuit = await _dataSyncRepository.GetCircuitByCodeAsync(code);
            if (circuit == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapCircuitToDto(circuit);
        }

        public async Task<int> GetCircuitsCountAsync()
        {
            return await _dataSyncRepository.GetCircuitsCountAsync();
        }
    }
}