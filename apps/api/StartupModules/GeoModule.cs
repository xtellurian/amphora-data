using Amphora.Api.Services.Azure;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amphora.Api.StartupModules
{
    public class GeoModule
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;

        public GeoModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.hostingEnvironment = env;
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureMapsOptions>(configuration.GetSection("AzureMaps"));

            if (hostingEnvironment.IsProduction() || configuration.IsPersistentStores())
            {
                services.AddTransient<IMapService, AzureMapService>();
            }
            else if (hostingEnvironment.IsDevelopment())
            {
                services.AddTransient<IMapService, InMemoryMapService>();
            }
        }
    }
}