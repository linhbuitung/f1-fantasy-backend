using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces
{
    public interface ICircuitService
    {
        Task<CircuitDto> AddCircuitAsync(CircuitDto circuitDto);

        void AddListCircuitsAsync(List<CircuitDto> circuitDtos);

        Task<CircuitDto> GetCircuitByIdAsync(Guid id);

        Task<CircuitDto> GetCircuitByCodeAsync(string code);
    }
}