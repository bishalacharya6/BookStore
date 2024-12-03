using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebApp.Utility
{
    public class EmailSender : IEmailSender
    {
        // Implementation of IEmailSender methods will go here
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
