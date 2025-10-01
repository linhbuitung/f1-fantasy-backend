using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations
{
    public class ConstructorService(IStaticDataRepository staticDataRepository, WooF1Context context)
        : IConstructorService
    {
        public async Task<ConstructorDto> AddConstructorAsync(ConstructorDto constructorDto)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                Constructor existingConstructor = await staticDataRepository.GetConstructorByCodeAsync(constructorDto.Code);
                if (existingConstructor != null)
                {
                    return null;
                }
                constructorDto = FixSpecialCountryCase(constructorDto);
                
                // Constructor API returns nationality, so we need check for nationality.
                Country country = await staticDataRepository.GetCountryByNationalityAsync(constructorDto.CountryId);
                if (country == null)
                {
                    throw new Exception($"Country with nationality {constructorDto.CountryId} not found");
                }
                constructorDto.CountryId = country.Id;
                
                Constructor constructor = StaticDataDtoMapper.MapDtoToConstructor(constructorDto);

                Constructor newConstructor = await staticDataRepository.AddConstructorAsync(constructor);

                // Additional operations that need atomicity (example: logging the event)
                await context.SaveChangesAsync();

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
            Constructor? constructor = await staticDataRepository.GetConstructorByIdAsync(id);
            if (constructor == null)
            {
                throw new NotFoundException($"Constructor with id {id} not found");
            }
            return StaticDataDtoMapper.MapConstructorToDto(constructor);
        }

        public async Task<ConstructorDto> GetConstructorByCodeAsync(string code)
        {
            Constructor? constructor = await staticDataRepository.GetConstructorByCodeAsync(code);
            if (constructor == null)
            {
                throw new NotFoundException($"Constructor with code {code} not found");
            }
            return StaticDataDtoMapper.MapConstructorToDto(constructor);
        }

        public async Task<List<ConstructorDto>> GetAllConstructorsAsync()
        {
            List<Constructor> constructors = await staticDataRepository.GetAllConstructorsAsync();
            return constructors.Select(StaticDataDtoMapper.MapConstructorToDto).ToList();
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