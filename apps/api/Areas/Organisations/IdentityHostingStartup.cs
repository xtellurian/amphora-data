﻿using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(Amphora.Api.Areas.Identity.IdentityHostingStartup))]
namespace Amphora.Api.Areas.Organisations
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}