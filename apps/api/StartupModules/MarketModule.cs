using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Azure;
using Amphora.Api.Services.Market;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Api.StartupModules
{
    public class MarketModule
    {
        public MarketModule(IConfiguration configuration, IHostingEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IMarketService, MarketService>();
            
            var key = Configuration.GetSection("AzureSearch")["PrimaryKey"];
            if(key != null)
            {
                // use azure search
                services.Configure<AzureSearchOptions>(Configuration.GetSection("AzureSearch"));
                services.AddTransient<ISearchService, AzureSearchService>();
            }
            else
            {
                // otherwise use basic search
            }
        }
    }
}