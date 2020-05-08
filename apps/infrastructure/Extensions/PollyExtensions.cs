using System;
using System.Net.Http;
using Amphora.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

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
            var policy = HttpPolicyExtensions
                .HandleTransientHttpError() // HttpRequestException, 5XX and 408
                .OrResult(response => (int)response.StatusCode == 429) // RetryAfter
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                });

            services.AddHttpClient(HttpClientNames.IdentityServerClient, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30); // fail faster
            })
           .AddPolicyHandler(policy);
        }
    }
}