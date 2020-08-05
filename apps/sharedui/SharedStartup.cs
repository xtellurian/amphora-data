using System.Net.Http;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Platform;
using Amphora.SharedUI.Services.Content;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amphora.SharedUI
{
    public class SharedStartup
    {
        protected IConfiguration Configuration { get; set; }
        protected IWebHostEnvironment HostingEnvironment { get; set; }
        protected EnvironmentInfo EnvironmentInfo { get; set; } = new EnvironmentInfo();
        protected void ConfigureSharedServices(IServiceCollection services)
        {
            System.Console.WriteLine($"Hosting Environment Name is {HostingEnvironment.EnvironmentName}");

            services.AddOptions(); // needed to load configuration from appsettings.json
            services.AddHealthChecks();
            services.AddTransient<IContentLoader, StaticContentLoader>(); // used for reading files from wwwroot
            services.AddTransient<IMarkdownToHtml, MarkdownToHtml>(); // used for reading files from wwwroot
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.Configure<EnvironmentInfo>(Configuration.GetSection("Environment"));
            Configuration.GetSection("Environment").Bind(EnvironmentInfo);
            EnvironmentInfo.IsDevelopment = HostingEnvironment.IsDevelopment();
        }

        protected void ConfigureSharedPipeline(IApplicationBuilder app)
        {
            // should come before other middleware. Because prod is behind a reverse proxy,
            // this is needed so the redirect_uri in identity server isn't http
            if (HostingEnvironment.IsProduction())
            {
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }

            app.MapWhen(
                context => context.Request.Method == HttpMethod.Get.Method &&
                    context.Request.Path.StartsWithSegments("/healthz"),
                builder => builder.UseHealthChecks("/healthz"));

            // app.UseHealthChecks("/healthz");
            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Home/StatusCode?code={0}");
                app.UseExceptionHandler("/Home/Error");

                // check if this is set, if yes, then don't disable (i.e. opposite of IfEnabled)
                if (!string.IsNullOrEmpty(Configuration["DisableHsts"]))
                {
                    System.Console.WriteLine("Enabling HSTS");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                else
                {
                    System.Console.WriteLine("HSTS is not enabled");
                }
            }
        }
    }
}