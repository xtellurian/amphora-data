using System;

namespace Amphora.Common.Extensions
{
    public static class UriExtensions
    {
        public static string ToStandardString(this Uri uri)
        {
            return uri.ToString().TrimEnd('/');
        }
    }
}