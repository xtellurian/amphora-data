using System;
using Amphora.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Amphora.Infrastructure.Extensions
{
    public static class PollyExtensions
    {
        public static void AddAzureMapsHttpClient(this IServiceCollection services)
        {
             services.AddHttpClient(HttpClientNames.AzureMaps)
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));
        }

        public static void AddIdentityServerHttpClient(this IServiceCollection services)
        {
             services.AddHttpClient(HttpClientNames.IdentityServerClient)
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));
        }
    }
}