using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Amphora.Api.Areas.Discover.DiscoverHostingStartup))]
namespace Amphora.Api.Areas.Discover
{
    public class DiscoverHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}