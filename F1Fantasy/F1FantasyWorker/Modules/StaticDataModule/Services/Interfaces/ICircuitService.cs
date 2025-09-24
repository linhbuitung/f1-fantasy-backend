using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces
{
    public interface ICircuitService
    {
        Task<CircuitDto> AddCircuitAsync(CircuitDto circuitDto);

        Task AddListCircuitsAsync(List<CircuitDto> circuitDtos);

        Task<CircuitDto> GetCircuitByIdAsync(int id);

        Task<CircuitDto> GetCircuitByCodeAsync(string code);
        Task<int> GetCircuitsCountAsync();
    }
}