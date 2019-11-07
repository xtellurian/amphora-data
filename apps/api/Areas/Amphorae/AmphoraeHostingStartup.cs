using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(Amphora.Api.Areas.Amphorae.AmphoraeHostingStartup))]
namespace Amphora.Api.Areas.Amphorae
{
    public class AmphoraeHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}