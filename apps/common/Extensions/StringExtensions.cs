using System;

namespace Amphora.Common.Extensions
{
    public static class StringExtensions
    {
        public static Uri ToUri(this string url, bool https = true)
        {
            if (url == null)
            {
                throw new NullReferenceException("Identity Base URL is null!");
            }

            url = url.TrimEnd('/');

            if (url.StartsWith("https://") || url.StartsWith("http://"))
            {
                return new Uri(url);
            }
            else if (https)
            {
                return new Uri($"https://{url}");
            }
            else
            {
                return new Uri($"http://{url}");
            }
        }
    }
}