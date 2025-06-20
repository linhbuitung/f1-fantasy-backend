﻿using F1Fantasy.Modules.StaticDataModule.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces
{
    public interface ICircuitService
    {
        Task<CircuitDto> AddCircuitAsync(CircuitDto circuitDto);

        void AddListCircuitsAsync(List<CircuitDto> circuitDtos);
    }
}