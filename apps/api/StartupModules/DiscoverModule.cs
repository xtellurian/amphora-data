using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Azure;
using Amphora.Api.Services.Basic;
using Amphora.Api.Services.Discover;
using Amphora.Common.Models.DataRequests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Api.StartupModules
{
    public class DiscoverModule
    {
        public DiscoverModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public IWebHostEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDiscoverService<DataRequestModel>, DataRequestDiscoveryService>();

            var key = Configuration.GetSection("AzureSearch")["PrimaryKey"];
            if (string.IsNullOrEmpty(key))
            {
                // otherwise use basic search
                services.AddTransient<ISearchService, BasicSearchService>();
            }
            else
            {
                // use azure search
                services.Configure<AzureSearchOptions>(Configuration.GetSection("AzureSearch"));
                services.AddTransient<ISearchService, AzureSearchService>();
                services.AddSingleton<IAzureSearchInitialiser, UnifiedSearchInitialiser>();
            }
        }
    }
}