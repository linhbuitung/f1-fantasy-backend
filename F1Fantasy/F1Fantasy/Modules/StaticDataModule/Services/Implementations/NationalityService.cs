using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations
{
    public class NationalityService : INationalityService
    {
        private readonly IStaticDataRepository _staticDataRepository;
        private readonly WooF1Context _context;

        public NationalityService(IStaticDataRepository staticDataRepository, WooF1Context context)
        {
            _staticDataRepository = staticDataRepository;
            _context = context;
        }

        public async Task<IEnumerable<NationalityDto>> GetAllNationalitiesAsync()
        {
            IEnumerable<Country> nationalities = await _staticDataRepository.GetAllNationalitiesAsync();
            return nationalities.Select(StaticDataDtoMapper.MapNationalityToDto).ToList();
        }

        public async Task<NationalityDto> GetNationalityByIdAsync(string id)
        {
            Country nationality = await _staticDataRepository.GetNationalityByIdAsync(id);
            if (nationality == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapNationalityToDto(nationality);
        }

        public async Task<NationalityDto> AddNationalityAsync(NationalityDto nationalityDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                NationalityDto existingNationality = await GetNationalityByIdAsync(nationalityDto.NationalityId);
                if (existingNationality != null)
                {
                    return null;
                }

                Country nationality = StaticDataDtoMapper.MapDtoToNationality(nationalityDto);
                Country newNationality = await _staticDataRepository.AddNationalityAsync(nationality);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StaticDataDtoMapper.MapNationalityToDto(newNationality);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating nationality: {ex.Message}");

                throw;
            }
        }
    }
}