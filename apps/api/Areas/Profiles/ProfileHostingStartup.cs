using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Amphora.Api.Areas.Profiles.ProfileHostingStartup))]
namespace Amphora.Api.Areas.Profiles
{
    public class ProfileHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}