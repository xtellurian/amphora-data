using System.Linq;
using Amphora.Common.Models.Activities;
using Microsoft.AspNetCore.Http;

namespace Amphora.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        private static string versionHeader = "amphora-client-version";

        /// <summary>
        /// Tries to read the version info from the http request headers.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="versionInfo">The extracted version info.</param>
        /// <returns> The contents of the stream as a byte array. </returns>
        public static bool TryReadClientVersion(this HttpRequest request, out VersionInfo versionInfo)
        {
            var versionString = "";
            try
            {
                if (request.Headers.ContainsKey(versionHeader))
                {
                    versionString = request.Headers[versionHeader].ToArray().FirstOrDefault();
                    var values = versionString?.Split('.')?.ToList();
                    // version string should be in the format 0.10.1
                    if (versionString != null || values?.Count >= 3)
                    {
                        versionInfo = new VersionInfo();
                        if (IsDigitsOnly(values[0]))
                        {
                            versionInfo.Major = int.Parse(values[0]);
                        }
                        else
                        {
                            return false;
                        }

                        if (IsDigitsOnly(values[1]))
                        {
                            versionInfo.Minor = int.Parse(values[1]);
                        }
                        else
                        {
                            return false;
                        }

                        if (IsDigitsOnly(values[2]))
                        {
                            versionInfo.Patch = int.Parse(values[2]);
                        }
                        else
                        {
                            return false;
                        }

                        // this is the only success ending.
                        return true;
                    }
                    else
                    {
                        versionInfo = null;
                        return false;
                    }
                }
                else
                {
                    versionInfo = null;
                    return false;
                }
            }
            catch (System.Exception)
            {
                versionInfo = null;
                return false;
            }
        }

        private static bool IsDigitsOnly(string str)
        {
            if (str == null)
            {
                return false;
            }

            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }
    }
}