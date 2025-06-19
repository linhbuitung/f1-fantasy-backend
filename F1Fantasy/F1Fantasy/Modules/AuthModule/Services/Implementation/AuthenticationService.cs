using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;
using F1Fantasy.Core.Configurations;
using F1Fantasy.Modules.AuthModule.Dtos;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace F1Fantasy.Modules.AuthModule.Services.Implementation
{
    public class AuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INationalityService _nationalityService;
        //private readonly IOptions<AuthConfiguration> _authOptions;

        //private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<ApplicationRole> _roleManager;

        public AuthenticationService(UserManager<ApplicationUser> userManager, INationalityService nationalityService)
        {
            _userManager = userManager;
            _nationalityService = nationalityService;
        }

        public async Task<ApplicationUserRegisterResponseDto> RegisterAsync(ApplicationUserRegisterDto registerDto)
        {
            ApplicationUser existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var nationality = await _nationalityService.GetNationalityByIdAsync(registerDto.NationalityId);
            if (nationality == null)
            {
                throw new InvalidOperationException("Invalid nationality ID.");
            }

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                DateOfBirth = registerDto.DateOfBirth,
                NationalityId = registerDto.NationalityId,
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("User registration failed");
            }

            result = await _userManager.AddToRoleAsync(newUser, AppRoles.Player);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to assign role to user");
            }

            ApplicationUserRegisterResponseDto response = new ApplicationUserRegisterResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Role = AppRoles.Player,
            };

            return response;
        }
    }
}