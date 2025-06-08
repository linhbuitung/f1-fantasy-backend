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

                Constructor constructor = StaticDataDtoMapper.MapDtoToConstructor(constructorDto);

                Constructor newConstructor = await _staticDataRepository.AddConstructorAsync(constructor);

                // Additional operations that need atomicity (example: logging the event)
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StaticDataDtoMapper.MapConstructorToDto(constructor);
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
    }
}