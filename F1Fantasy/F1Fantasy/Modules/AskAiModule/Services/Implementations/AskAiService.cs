using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Modules.AskAiModule.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace F1Fantasy.Modules.AskAiModule.Services.Implementations;

public class AskAiService(UserManager<ApplicationUser> userManager) : IAskAIService
{
    public async Task AddAskAiCreditAsync(int userId)
    {
        await AddAskAiCreditAsync(userId.ToString());
    }

    // string userId will be converted to int 
    public async Task AddAskAiCreditAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.AskAiCredits += 1;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to add AI credit");
        }
    }
}