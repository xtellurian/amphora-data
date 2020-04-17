using System;
using System.Collections.Generic;
using Amphora.Common.Extensions;

namespace Amphora.Common
{
    public static class AmphoraHost
    {
        public static bool IsDevelopment { get; set; }
        private static string? environmentName;
        private static string? appName;
        private static List<string> appNames = new List<string> { "app", "identity" };
        private static List<string> environmentNames = new List<string> { "develop", "master", "prod" };

        public static void SetEnvironmentName(string name)
        {
            if (environmentNames.Contains(name))
            {
                environmentName = name;
            }
            else
            {
                throw new ArgumentException($"{name} is an invalid Environment Name");
            }
        }

        public static void SetAppName(string name)
        {
            if (appNames.Contains(name))
            {
                appName = name;
            }
            else
            {
                throw new ArgumentException($"{name} is an invalid App Name");
            }
        }

        public static string GetHost()
        {
            return GetHostUri().ToStandardString();
        }

        public static Uri GetHostUri()
        {
            if (appName == null)
            {
                throw new InvalidOperationException("App Name is unknown");
            }

            if (environmentName == null)
            {
                throw new InvalidOperationException("Environment Name is unknown");
            }

            if (IsDevelopment)
            {
                switch (appName)
                {
                    case "identity":
                        return new Uri("http://localhost:6500");
                    case "app":
                        return new Uri("https://localhost:5001");
                    default:
                        throw new InvalidOperationException("Unknown App Name");
                }
            }
            else
            {
                try
                {
                    if (environmentName == "prod")
                    {
                        return new Uri($"https://{appName}.amphoradata.com");
                    }
                    else
                    {
                        return new Uri($"https://{environmentName}.{appName}.amphoradata.com");
                    }
                }
                catch (System.UriFormatException)
                {
                    Console.WriteLine($"Bad Format for env {environmentName}, appName: {appName}");
                    throw;
                }
            }
        }
    }
}