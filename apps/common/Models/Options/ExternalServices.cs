using System;

namespace Amphora.Common.Models.Options
{
    public class ExternalServices
    {
        public string? IdentityBaseUrl { get; set; }
        public string? WebAppBaseUrl { get; set; }

        public Uri IdentityUri()
        {
            return ToUri(IdentityBaseUrl);
        }

        public Uri WebAppUri()
        {
            return ToUri(WebAppBaseUrl);
        }

        private Uri ToUri(string? url)
        {
            if (url == null)
            {
                throw new NullReferenceException("Identity Base URL is null!");
            }

            if (url.StartsWith("https://") || url.StartsWith("http://"))
            {
                return new Uri(url);
            }
            else
            {
                return new Uri($"https://{url}");
            }
        }
    }
}