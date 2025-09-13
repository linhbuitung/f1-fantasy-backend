using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces
{
    public interface IConstructorService
    {
        Task<ConstructorDto> AddConstructorAsync(ConstructorDto constructorDto);

        void AddListConstructorsAsync(List<ConstructorDto> constructorDtos);

        Task<ConstructorDto> GetConstructorByIdAsync(int id);

        Task<ConstructorDto> GetConstructorByCodeAsync(string code);

        Task<int> GetConstructorsCountAsync();
    }
}