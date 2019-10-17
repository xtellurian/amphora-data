using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(Amphora.Areas.Identity.IdentityHostingStartup))]
namespace Amphora.Areas.Organisations
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