using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations
{
    public class CircuitService(IStaticDataRepository staticDataRepository, WooF1Context context)
        : ICircuitService
    {
        public async Task<CircuitDto> AddCircuitAsync(CircuitDto circuitDto)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                Circuit? existingCircuit = await staticDataRepository.GetCircuitByCodeAsync(circuitDto.Code);
                if (existingCircuit != null)
                {
                    return null;
                }
                
                // Circuit API returns country name, so we need check for short name.
                Country? country = await staticDataRepository.GetCountryByShortNameAsync(circuitDto.CountryId);
                if (country == null)
                {
                    throw new NotFoundException($"Country with name {circuitDto.CountryId} not found");
                }
                circuitDto.CountryId = country.Id;
                
                Circuit circuit = StaticDataDtoMapper.MapDtoToCircuit(circuitDto);

                Circuit newDircuit = await staticDataRepository.AddCircuitAsync(circuit);

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
            Circuit? circuit = await staticDataRepository.GetCircuitByIdAsync(id);
            if (circuit == null)
            {
                throw new NotFoundException($"Circuit with id {id} not found");
            }
            return StaticDataDtoMapper.MapCircuitToDto(circuit);
        }

        public async Task<CircuitDto> GetCircuitByCodeAsync(string code)
        {
            Circuit? circuit = await staticDataRepository.GetCircuitByCodeAsync(code);
            if (circuit == null)
            {
                throw new NotFoundException($"Circuit with code {code} not found");
            }
            return StaticDataDtoMapper.MapCircuitToDto(circuit);
        }
    }
}