using System;

namespace Amphora.Infrastructure.Models.Options
{
    public class SendGridOptions
    {
        public string? ApiKey { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public bool? Suppress { get; set; }
        public string? BccAddress { get; set; } = "internal@amphoradata.com";

        public void ThrowIfInvalid()
        {
            if (string.IsNullOrEmpty(FromEmail))
            {
                throw new NullReferenceException($"{nameof(FromEmail)} cannot be null or empty");
            }
        }
    }
}