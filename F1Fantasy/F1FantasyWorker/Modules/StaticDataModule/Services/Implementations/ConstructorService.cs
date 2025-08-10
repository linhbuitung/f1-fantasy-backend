using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations
{
    public class ConstructorService : IConstructorService
    {
        private readonly IStaticDataRepository _staticDataRepository;
        private readonly WooF1Context _context;

        public ConstructorService(IStaticDataRepository staticDataRepository, WooF1Context context)
        {
            _staticDataRepository = staticDataRepository;
            _context = context;
        }

        public async Task<ConstructorDto> AddConstructorAsync(ConstructorDto constructorDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Constructor existingConstructor = await _staticDataRepository.GetConstructorByCodeAsync(constructorDto.Code);
                if (existingConstructor != null)
                {
                    return null;
                }
                constructorDto = FixSpecialCountryCase(constructorDto);
                
                // Constructor API returns nationality, so we need check for nationality.
                Country country = await _staticDataRepository.GetCountryByNationalitityAsync(constructorDto.CountryId);
                if (country == null)
                {
                    throw new Exception($"Country with nationality {constructorDto.CountryId} not found");
                }
                constructorDto.CountryId = country.Id;
                
                Constructor constructor = StaticDataDtoMapper.MapDtoToConstructor(constructorDto);

                Constructor newConstructor = await _staticDataRepository.AddConstructorAsync(constructor);

                // Additional operations that need atomicity (example: logging the event)
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StaticDataDtoMapper.MapConstructorToDto(newConstructor);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating constructor: {ex.Message}");

                throw;
            }
        }

        public async void AddListConstructorsAsync(List<ConstructorDto> constructorDtos)
        {
            foreach (var constructor in constructorDtos)
            {
                await AddConstructorAsync(constructor);
            }
        }

        //get
        public async Task<ConstructorDto> GetConstructorByIdAsync(int id)
        {
            Constructor constructor = await _staticDataRepository.GetConstructorByIdAsync(id);
            return StaticDataDtoMapper.MapConstructorToDto(constructor);
        }

        public async Task<ConstructorDto> GetConstructorByCodeAsync(string code)
        {
            Constructor constructor = await _staticDataRepository.GetConstructorByCodeAsync(code);
            return StaticDataDtoMapper.MapConstructorToDto(constructor);
        }
        
        private ConstructorDto FixSpecialCountryCase(ConstructorDto constructorDto)
        {
            if (constructorDto.CountryId.Equals("East German"))
            {
                constructorDto.CountryId = "German";
            }
            else if (constructorDto.CountryId.Equals("Rhodesian"))
            {
                constructorDto.CountryId = "Zimbabwean";
            }
            
            return constructorDto;
        }
    }
}