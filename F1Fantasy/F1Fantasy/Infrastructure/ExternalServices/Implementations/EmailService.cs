using F1Fantasy.Core.Common;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using F1Fantasy.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace F1Fantasy.Infrastructure.ExternalServices.Implementations
{
    public class EmailService : IEmailSender<ApplicationUser>
    {
        private readonly ILogger<EmailService> _logger;
        private readonly MailSettings _emailSettings;

        public EmailService(ILogger<EmailService> logger, IOptions<MailSettings> mailSettings)
        {
            _logger = logger;
            _emailSettings = mailSettings.Value;
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);

                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Enable HTML
                };
                mailMessage.To.Add(to);
                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            var subject = "Confirm your email";
            var body = $@"<p>Please confirm your account by clicking this link:</p><p><a href=""{confirmationLink}"">Confirm Email</a></p><p>Or copy and paste this URL into your browser:<br>{confirmationLink}</p>";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            var subject = "Reset your password";
            var body = $@"<p>Reset your password by clicking this link:</p><p><a href=""{resetLink}"">Reset Password</a></p><p>Or copy and paste this URL into your browser:<br>{resetLink}</p>";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            var subject = "Your password reset code";
            var body = $"Your password reset code is: <b>{resetCode}</b>";
            await SendEmailAsync(email, subject, body);
        }
    }
}