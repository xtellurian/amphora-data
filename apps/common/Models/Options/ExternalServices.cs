using System;
using Amphora.Common.Extensions;

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
                throw new NullReferenceException("IdentityBaseUrl cannot be null");
            }

            return IdentityBaseUrl.ToUri();
        }

        public Uri WebAppUri()
        {
            if (WebAppBaseUrl == null)
            {
                throw new NullReferenceException("WebAppBaseUrl cannot be null");
            }

            return WebAppBaseUrl.ToUri();
        }
    }
}