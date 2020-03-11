using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Infrastructure.Database.EFCoreProviders
{
    public static class InMemoryProvider
    {
        public static void UseInMemory<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
                {
                    options.UseInMemoryDatabase("Amphora");
                    options.UseLazyLoadingProxies();
                });
        }
    }
}