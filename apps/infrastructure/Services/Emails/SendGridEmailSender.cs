using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Infrastructure.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Amphora.Infrastructure.Services.Emails
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class SendGridEmailSender : IEmailSender
    {
        private readonly ILogger<SendGridEmailSender> logger;
        private SendGridOptions options;
        public IEmailGenerator Generator { get; }

        public SendGridEmailSender(ILogger<SendGridEmailSender> logger, IOptionsMonitor<SendGridOptions> options, IEmailGenerator generator)
        {
            this.options = options.CurrentValue;
            this.logger = logger;
            Generator = generator;
            options.CurrentValue?.ThrowIfInvalid();
        }

        public async Task<bool> SendEmailAsync(IEmail email)
        {
            try
            {
                if (email.Recipients == null || email.Recipients.Where(_ => _.Email != null).Count() == 0)
                {
                    logger.LogWarning($"Refusing to send {email?.GetType()} email with no valid recipients.");
                    return false;
                }

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

                msg.SetSubject(email.Subject);
                msg.AddContent(MimeType.Html, email.HtmlContent);
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
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("message", nameof(email));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("message", nameof(subject));
            }

            if (string.IsNullOrEmpty(htmlMessage))
            {
                throw new ArgumentException("message", nameof(htmlMessage));
            }

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
            if (!string.IsNullOrEmpty(options?.BccAddress))
            {
                msg.AddBcc(options?.BccAddress);
            }

            if (string.IsNullOrEmpty(options?.ApiKey))
            {
                logger.LogError($"Send Grid API Key not provided. Email not sent.");
                if (options?.Suppress == false)
                {
                    throw new System.ArgumentNullException($"Send Grid API Key not provided. Email not sent.");
                }
            }
            else if (options?.Suppress == true)
            {
                logger.LogWarning($"Email suppressed via configuration.");
                logger.LogTrace(JsonConvert.SerializeObject(msg));
            }
            else
            {
                logger.LogInformation($"Sending email from {msg.From}");
                var client = new SendGridClient(options?.ApiKey);
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