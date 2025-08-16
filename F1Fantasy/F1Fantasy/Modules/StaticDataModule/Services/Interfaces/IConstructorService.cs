using F1Fantasy.Modules.StaticDataModule.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces
{
    public interface IConstructorService
    {
        Task<ConstructorDto> AddConstructorAsync(ConstructorDto constructorDto);

        void AddListConstructorsAsync(List<ConstructorDto> constructorDtos);

        Task<ConstructorDto> GetConstructorByIdAsync(int id);

        Task<ConstructorDto> GetConstructorByCodeAsync(string code);
    }
}