using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Azure;
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

            if (hostingEnvironment.IsProduction() || configuration["PersistentStores"] == "true")
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