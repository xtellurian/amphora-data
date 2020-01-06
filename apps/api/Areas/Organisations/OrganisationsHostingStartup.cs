using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Amphora.Api.Areas.Organisations.OrganisationsHostingStartup))]
namespace Amphora.Api.Areas.Organisations
{
    public class OrganisationsHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}