﻿using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations
{
    public class CircuitService : ICircuitService
    {
        private readonly IStaticDataRepository _staticDataRepository;
        private readonly WooF1Context _context;

        public CircuitService(IStaticDataRepository staticDataRepository, WooF1Context context)
        {
            _staticDataRepository = staticDataRepository;
            _context = context;
        }

        public async Task<CircuitDto> AddCircuitAsync(CircuitDto circuitDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Circuit existingCircuit = await _staticDataRepository.GetCircuitByCodeAsync(circuitDto.Code);
                if (existingCircuit != null)
                {
                    return null;
                }

                Circuit circuit = StaticDataDtoMapper.MapDtoToCircuit(circuitDto);

                Circuit newDircuit = await _staticDataRepository.AddCircuitAsync(circuit);

                // Additional operations that need atomicity (example: logging the event)
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StaticDataDtoMapper.MapCircuitToDto(circuit);
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
            Circuit circuit = await _staticDataRepository.GetCircuitByIdAsync(id);
            if (circuit == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapCircuitToDto(circuit);
        }

        public async Task<CircuitDto> GetCircuitByCodeAsync(string code)
        {
            Circuit circuit = await _staticDataRepository.GetCircuitByCodeAsync(code);
            if (circuit == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapCircuitToDto(circuit);
        }
    }
}