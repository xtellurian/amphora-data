using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.SharedUI
{
    public class SharedStartup
    {
        protected IConfiguration Configuration { get; set; }
        protected IWebHostEnvironment HostingEnvironment { get; set; }
        protected void ConfigureSharedServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        protected void ConfigureSharedPipeline(IApplicationBuilder app)
        {
            app.UseHealthChecks("/healthz");
            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }
    }
}