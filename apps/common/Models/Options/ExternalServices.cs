using System;

namespace Amphora.Common.Models.Options
{
    public class ExternalServices
    {
        public string? IdentityBaseUrl { get; set; }
        public string? WebAppBaseUrl { get; set; }

        public Uri IdentityUri()
        {
            if (IdentityBaseUrl == null)
            {
                throw new NullReferenceException("Identity Base URL is null!");
            }

            if (IdentityBaseUrl.StartsWith("https://") || IdentityBaseUrl.StartsWith("http://"))
            {
                return new Uri(IdentityBaseUrl);
            }
            else
            {
                return new Uri($"https://{IdentityBaseUrl}");
            }
        }
    }
}