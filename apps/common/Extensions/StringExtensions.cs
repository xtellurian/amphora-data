using System;

namespace Amphora.Common.Extensions
{
    public static class StringExtensions
    {
        public static Uri ToUri(this string url)
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
            else
            {
                return new Uri($"https://{url}");
            }
        }
    }
}