using Amphora.Infrastructure.Models.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amphora.Infrastructure.Database.EFCoreProviders
{
    public static class SqlServerProvider
    {
        private const string DefaultHost = "127.0.0.1";
        private const int DefaultPort = 1433;
        private const string DefaultDatabase = "dbo";
        private const string DefaultUser = "SA";
        private const string DefaultPassword = "YourStrong@Passw0rd";
        private static string GetConnectionString(string? host, int? port, string? database, string? user, string? password) =>
            $"Server={host ?? DefaultHost},"
            + $"{port ?? DefaultPort};"
            + $"Database={database ?? DefaultDatabase};"
            + $"User Id={user ?? DefaultUser};"
            + $"Password={password ?? DefaultPassword}";

        public static void UseSqlServer<TContext>(this IServiceCollection services, SqlServerOptions? options) where TContext : DbContext
        {
            string? connectionString = string.Empty;
            if (options?.ConnectionString != null)
            {
                connectionString = options?.ConnectionString;
            }
            else
            {
                connectionString = GetConnectionString(options?.Host, options?.Port, options?.Database, options?.User, options?.Password);
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new System.ArgumentException("Connection String could not be constructed");
            }

            services.AddDbContext<TContext>(optionsBuilder =>
               {
                   optionsBuilder.UseSqlServer(connectionString);
                   optionsBuilder.UseLazyLoadingProxies();
               });

            var msg = $"Using Sql Server, Host:{options?.Host ?? DefaultHost} on port: {options?.Port ?? DefaultPort}";

            System.Console.WriteLine(msg);
        }

        public static void MigrateSql<TContext>(this IApplicationBuilder app) where TContext : DbContext
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<TContext>();
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
        }
    }
}