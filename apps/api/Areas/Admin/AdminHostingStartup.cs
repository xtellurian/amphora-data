using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(Amphora.Api.Areas.Amphorae.AdminHostingStartup))]
namespace Amphora.Api.Areas.Amphorae
{
    public class AdminHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}