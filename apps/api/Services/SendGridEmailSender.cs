using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Emails;
using Amphora.Api.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        public async Task<bool> SendEmailAsync(IEmail email)
        {
            try
            {
                var msg = new SendGridMessage();
                msg.SetFrom(new EmailAddress(options.FromEmail, options.FromName));
                foreach (var r in email.Recipients)
                {
                    if (r.Email != null)
                    {
                        msg.AddTo(new EmailAddress(r.Email, r.FullName));
                    }
                    else
                    {
                        logger.LogWarning($"Cannot send email to user without an email");
                    }
                }

                msg.SetTemplateId(email.SendGridTemplateId);

                msg.SetTemplateData(email);
                await TrySendMessage(msg);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to send email, {ex.Message}", ex);
                return false;
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
                logger.LogError($"Send Grid API Key not provided. Email not sent.");
                if (options.Suppress == false)
                {
                    throw new System.ArgumentNullException($"Send Grid API Key not provided. Email not sent.");
                }
            }
            else if (options.Suppress == true)
            {
                logger.LogWarning($"Email suppressed via configuration.");
                logger.LogTrace(JsonConvert.SerializeObject(msg));
            }
            else
            {
                logger.LogInformation($"Sending email from {msg.From}");
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