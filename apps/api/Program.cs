﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Amphora.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // await CreateWebHostBuilder(args).Build().RunAsync();
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // first we get the KeyVault ID from the local condig
                    config.AddEnvironmentVariables();
                    var settings = config.Build();
                    config = KeyVaultConfigProvider.Configure(config, settings);
                    // then we use the Azure App Config Connection String in the KeyVault to connect
                    settings = config.Build(); // build again for appsettings

                    // disable Azure App Config. I'm not using it and its fucking my tests, due to rate limiting
                    // config = AzureAppConfigurationConfigProvider.Configure(config, settings);

                    config.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            { "Version", Amphora.Api.ApiVersion.CurrentVersion.ToSemver() },
                        });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        // Set properties and call methods on options
                    })
                    .UseStartup<Startup>();
                });
    }
}
