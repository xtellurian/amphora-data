using Amphora.Common.EntityFramework.TypeConfiguration.ApplicationsContext;
using Amphora.Common.Models.Applications;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Infrastructure.Database.Contexts
{
    public class ApplicationsContext : DbContext
    {
        public ApplicationsContext(DbContextOptions<ApplicationsContext> options)
           : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationModelTypeConfiguration).Assembly,
           _ => _.Namespace.StartsWith(typeof(ApplicationModelTypeConfiguration).Namespace));
        }

        public DbSet<ApplicationModel> Applications { get; set; } = null!;
        public DbSet<ApplicationLocationModel> Locations { get; set; } = null!;
    }
}