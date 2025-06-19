using F1Fantasy.Modules.StaticDataModule.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces
{
    public interface INationalityService
    {
        Task<NationalityDto> GetNationalityByIdAsync(string id);

        Task<IEnumerable<NationalityDto>> GetAllNationalitiesAsync();

        Task<NationalityDto> AddNationalityAsync(NationalityDto nationalityDto);
    }
}