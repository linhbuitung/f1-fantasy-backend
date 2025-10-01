using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Configs;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations
{
    public class ConstructorService(IDataSyncRepository dataSyncRepository, WooF1Context context)
        : IConstructorService
    {
        public async Task<ConstructorDto> AddConstructorAsync(ConstructorDto constructorDto)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                Constructor existingConstructor = await dataSyncRepository.GetConstructorByCodeAsync(constructorDto.Code);
                if (existingConstructor != null)
                {
                    return null;
                }
                constructorDto = FixSpecialCountryCase(constructorDto);
                
                // Constructor API returns nationality, so we need check for nationality.
                Country country = await dataSyncRepository.GetCountryByNationalitityAsync(constructorDto.CountryId);
                if (country == null)
                {
                    throw new Exception($"Country with nationality {constructorDto.CountryId} not found");
                }
                constructorDto.CountryId = country.Id;
                
                Constructor constructor = StaticDataDtoMapper.MapDtoToConstructor(constructorDto);

                Constructor newConstructor = await dataSyncRepository.AddConstructorAsync(constructor);

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

        public async Task AddListConstructorsAsync(List<ConstructorDto> constructorDtos)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var existingCountries = await dataSyncRepository.GetAllCountriesAsync();
                var existingConstructorCodes = await dataSyncRepository.GetAllConstructorCodesAsync();
                var newConstructors = new List<Constructor>();
                foreach (var constructorDto in constructorDtos)
                {
                    if (existingConstructorCodes.Contains(constructorDto.Code))
                    {
                        continue; // Skip existing constructors
                    }

                    var fixedConstructorDto = FixSpecialCountryCase(constructorDto);
                    
                    // Constructor API returns nationality, so we need check for nationality.
                    if (!existingCountries.Exists(c => c.Nationalities.Contains(fixedConstructorDto.CountryId)))
                    {
                        throw new Exception($"Country with nationality {fixedConstructorDto.CountryId} not found");
                    }
                    fixedConstructorDto.CountryId = existingCountries.First(c => c.Nationalities.Contains(fixedConstructorDto.CountryId)).Id;
                
                    Constructor constructor = StaticDataDtoMapper.MapDtoToConstructor(fixedConstructorDto);

                    newConstructors.Add(constructor);
                }

                var newConstructorsReturned = await dataSyncRepository.AddListConstructorsAsync(newConstructors);
                // Additional operations that need atomicity (example: logging the event)
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating constructor: {ex.Message}");

                throw;
            }
        }

        //get
        public async Task<ConstructorDto> GetConstructorByIdAsync(int id)
        {
            Constructor constructor = await dataSyncRepository.GetConstructorByIdAsync(id);
            return StaticDataDtoMapper.MapConstructorToDto(constructor);
        }

        public async Task<ConstructorDto> GetConstructorByCodeAsync(string code)
        {
            Constructor constructor = await dataSyncRepository.GetConstructorByCodeAsync(code);
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

        public async Task<int> GetConstructorsCountAsync()
        {
            return await dataSyncRepository.GetConstructorsCountAsync();
        }
    }
}