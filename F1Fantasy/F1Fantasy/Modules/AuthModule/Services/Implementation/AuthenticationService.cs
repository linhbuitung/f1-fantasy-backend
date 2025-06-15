using F1Fantasy.Core.Common;
using F1Fantasy.Modules.AuthModule.Dtos;
using Microsoft.AspNetCore.Identity;

namespace F1Fantasy.Modules.AuthModule.Services.Implementation
{
    public class AuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUserResponseDto> RegisterAsync(ApplicationUserRegisterDto registerDto)
        {
            ApplicationUser existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            return null;
        }
    }
}