using F1Fantasy.Core.Common;
using Microsoft.AspNetCore.Identity;

namespace F1Fantasy.Infrastructure.ExternalServices.Implementations
{
    public class EmailService : IEmailSender<ApplicationUser>
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            _logger.LogInformation("Confirmation email to {Email}: {Link}", email, confirmationLink);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            _logger.LogInformation("Password reset email to {Email}: {Link}", email, resetLink);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            _logger.LogInformation("Password reset code to {Email}: {Code}", email, resetCode);
            return Task.CompletedTask;
        }
    }
}