using Amphora.Common.EntityFramework;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Identity.EntityFramework
{
    public class IdentityContext : CommonContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // this is due to migrating
            modelBuilder.HasDefaultContainer("AmphoraContext");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Amphora.Common.EntityFramework.CommonContext).Assembly);

            // modelBuilder.Entity<<IdentityUserClaim<string>>>().ToTable("  ");
            // modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            // modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            // modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            // modelBuilder.Entity<ApplicationUser>().ToTable("Users");
        }
    }
}