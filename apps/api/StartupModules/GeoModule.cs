using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Azure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Api.StartupModules
{
    public class GeoModule
    {
        private readonly IHostingEnvironment HostingEnvironment;
        private readonly IConfiguration Configuration;

        public GeoModule(IConfiguration configuration, IHostingEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureMapsOptions>(Configuration.GetSection("AzureMaps"));

            if (HostingEnvironment.IsProduction() || Configuration["PersistentStores"] == "true")
            {
                services.AddTransient<IMapService, AzureMapService>();
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                services.AddTransient<IMapService, InMemoryMapService>();
            }
            
        }
    }
}