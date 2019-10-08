using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Amphora.Api.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class SendGridEmailSender : IEmailSender
    {
        private readonly ILogger<SendGridEmailSender> logger;
        private SendGridOptions options;

        public SendGridEmailSender(ILogger<SendGridEmailSender> logger, IOptionsMonitor<SendGridOptions> options)
        {
            this.options = options.CurrentValue;
            this.logger = logger;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var msg = new SendGridMessage();

            msg.SetFrom(new EmailAddress(options.FromEmail, options.FromName));

            var recipients = new List<EmailAddress>
            {
                new EmailAddress(email),
            };
            msg.AddTos(recipients);

            msg.SetSubject(subject);

            msg.AddContent(MimeType.Html, htmlMessage);

            if(string.IsNullOrEmpty(options.ApiKey))
            {
                logger.LogWarning($"Send Grid API Key not provided. Email not sent to {email}");
                logger.LogInformation($"Tried to send: {htmlMessage} to {email}");
            }
            else
            {
                logger.LogInformation($"Sending email to {email}");
                var client = new SendGridClient(options.ApiKey);
                var response = await client.SendEmailAsync(msg);
                var body = await response.Body.ReadAsStringAsync();
                if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    logger.LogCritical(body);
                    throw new ApplicationException("Failed to send confirmation email");
                }
            }
        }
    }
}