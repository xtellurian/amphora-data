using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Emails;
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

        public async Task SendEmailAsync(IEmail email)
        {
            try
            {
                var msg = new SendGridMessage();
                msg.SetFrom(new EmailAddress(options.FromEmail, options.FromName));
                msg.AddTo(new EmailAddress(email.ToEmail, email.ToName));
                msg.SetTemplateId(email.SendGridTemplateId);

                msg.SetTemplateData(email);
                await TrySendMessage(msg);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to send email, {ex.Message}", ex);
            }
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
            await TrySendMessage(msg);
        }

        private async Task TrySendMessage(SendGridMessage msg)
        {
            if (string.IsNullOrEmpty(options.ApiKey))
            {
                logger.LogWarning($"Send Grid API Key not provided. Email not sent.");
            }
            else
            {
                logger.LogInformation($"Sending email to {msg.From}");
                var client = new SendGridClient(options.ApiKey);
                var response = await client.SendEmailAsync(msg);
                var body = await response.Body.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    logger.LogCritical(body);
                    throw new ApplicationException("Failed to send confirmation email");
                }
            }
        }
    }
}