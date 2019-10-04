using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(Amphora.Areas.Amphorae.AmphoraeHostingStartup))]
namespace Amphora.Areas.Amphorae
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