using System.Linq;
using Amphora.Api.AspNet.NSwag;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace Amphora.Api.StartupModules
{
    public class OpenApiModule
    {
        public OpenApiModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        private const string Title = "Amphora Data";
        private const string Description = @"
            Connect information in real time with Amphora Data.
            
            Learn more at https://docs.amphoradata.com";

        public IWebHostEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenApiDocument(document => // add OpenAPI v3 document
            {
                document.DocumentName = "v1";
                document.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Bearer {your JWT token}."
                });

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

                document.FlattenInheritanceHierarchy = true;
                document.Description = Description;
                document.Title = Title;
                document.Version = ApiVersion.CurrentVersion.ToSemver();
                document.SchemaProcessors.Add(new VariableSchemaProcessor());
            });

            services.AddSwaggerDocument(document => // add a Swagger (OpenAPI v2) doc
            {
                document.DocumentName = "v2";
                document.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Bearer {your JWT token}."
                });
                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

                // lets at least flatten for the older version of swagger
                document.FlattenInheritanceHierarchy = true;
                document.Description = Description;
                document.Title = Title;
                document.Version = ApiVersion.CurrentVersion.ToSemver();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(settings => { }); // serve Swagger UI
                                                // app.UseReDoc(); // serve ReDoc UIÎ
        }
    }
}