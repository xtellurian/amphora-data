using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(Amphora.Areas.Identity.IdentityHostingStartup))]
namespace Amphora.Areas.Identity
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