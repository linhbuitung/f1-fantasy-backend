using System.Text;
using System.Text.Encodings.Web;
using F1Fantasy.Core.Auth;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using F1Fantasy.Core.Common;
using Microsoft.AspNetCore.Identity.Data;

namespace F1Fantasy.Modules.AuthModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IEmailSender<ApplicationUser> _emailSender;
    private readonly IOptionsMonitor<BearerTokenOptions> _bearerTokenOptions;
    private readonly TimeProvider _timeProvider;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IEmailSender<ApplicationUser> emailSender,
        IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
        TimeProvider timeProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _bearerTokenOptions = bearerTokenOptions;
        _timeProvider = timeProvider;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registration)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var email = registration.Email;
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, registration.Password);
        if (!result.Succeeded)
            return ValidationProblem(result.ToString());

        // Ensure Player role exists
        if (!await _roleManager.RoleExistsAsync("Player"))
            await _roleManager.CreateAsync(new ApplicationRole { Name = "Player" });
        await _userManager.AddToRoleAsync(user, "Player");

        await SendConfirmationEmailAsync(user, email);
        return Ok();
    }

    [HttpGet("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code, [FromQuery] string? changedEmail)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Unauthorized();
        try { code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)); }
        catch { return Unauthorized(); }
        IdentityResult result;
        if (string.IsNullOrEmpty(changedEmail))
            result = await _userManager.ConfirmEmailAsync(user, code);
        else {
            result = await _userManager.ChangeEmailAsync(user, changedEmail, code);
            if (result.Succeeded)
                result = await _userManager.SetUserNameAsync(user, changedEmail);
        }
        if (!result.Succeeded) return Unauthorized();
        return Content("Thank you for confirming your email.");
    }

    [HttpPost("resendConfirmationEmail")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest resendRequest)
    {
        var user = await _userManager.FindByEmailAsync(resendRequest.Email);
        if (user == null) return Ok();
        await SendConfirmationEmailAsync(user, resendRequest.Email);
        return Ok();
    }
    
    private async Task SendConfirmationEmailAsync(ApplicationUser user, string email)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action(
            nameof(ConfirmEmail),
            "Auth",
            new { userId = user.Id, code },
            protocol: HttpContext.Request.Scheme);
        await _emailSender.SendConfirmationLinkAsync(user, email, callbackUrl);
    }
}