using System;

namespace Amphora.Infrastructure.Models.Options
{
    public class SendGridOptions
    {
        public string? ApiKey { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public bool? Suppress { get; set; }

        public void ThrowIfInvalid()
        {
            if (string.IsNullOrEmpty(FromEmail))
            {
                throw new NullReferenceException($"{nameof(FromEmail)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(ApiKey))
            {
                throw new NullReferenceException($"{nameof(ApiKey)} cannot be null or empty");
            }
        }
    }
}