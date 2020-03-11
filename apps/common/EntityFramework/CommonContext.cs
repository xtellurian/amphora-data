using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Common.EntityFramework
{
    public abstract class CommonContext : IdentityDbContext<ApplicationUser>
    {
        public CommonContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<AmphoraModel> Amphorae { get; set; } = null!;
        public DbSet<OrganisationModel> Organisations { get; set; } = null!;
        public DbSet<DataRequestModel> DataRequests { get; set; } = null!;
        public DbSet<PurchaseModel> Purchases { get; set; } = null!;
        public DbSet<CommissionModel> Commissions { get; set; } = null!;
        public DbSet<InvitationModel> Invitations { get; set; } = null!;
        public DbSet<RestrictionModel> Restrictions { get; set; } = null!;
    }
}