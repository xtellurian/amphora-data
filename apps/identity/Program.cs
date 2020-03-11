// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using Amphora.Common.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Amphora.Identity
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
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
                    config = AzureAppConfigurationConfigProvider.Configure(config, settings);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
