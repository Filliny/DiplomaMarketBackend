using DiplomaMarketBackend.Helpers;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Text.RegularExpressions;
using WebShopApp.Abstract;

namespace DiplomaMarketBackend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<EmailSettings> _options;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration configuration, IOptions<EmailSettings> options,ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _options = options;
            _logger = logger;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string? fromEmail = _options.Value.SenderEmail;
            string? fromName = _options.Value.SenderName;
            string? apiKey = _options.Value.ApiKey;
            var sendGridClient = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(email);
            var plainTextContent = Regex.Replace(htmlMessage, "<[^>]*>", "");
            var msg = MailHelper.CreateSingleEmail(from, to, subject,
            plainTextContent, htmlMessage);
            var response = await sendGridClient.SendEmailAsync(msg);

            _logger.LogInformation(plainTextContent, response);

        }
    }
}
