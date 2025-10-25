using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Shared.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces
{
    public interface ICircuitService
    {
        Task<CircuitDto> AddCircuitAsync(CircuitDto circuitDto);

        void AddListCircuitsAsync(List<CircuitDto> circuitDtos);

        Task<CircuitDto> GetCircuitByIdAsync(int id);

        Task<CircuitDto> GetCircuitByCodeAsync(string code);
        
        Task<PagedResult<CircuitDto>> SearchCircuitsAsync(string query, int pageNum, int pageSize);

    }
}