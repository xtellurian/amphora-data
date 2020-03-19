using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Infrastructure
{
    public abstract class StartupBase
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure(IApplicationBuilder app)
        {

        }
    }
}