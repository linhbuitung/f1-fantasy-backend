using F1Fantasy.Modules.AuthModule.Dtos;

namespace F1Fantasy.Modules.AuthModule.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApplicationUserRegisterResponseDto> RegisterAsync(ApplicationUserRegisterDto registerDto);

        Task<ApplicationUserTokenResponseDto> LoginAsync(ApplicationUserLoginDto loginDto);
    }
}