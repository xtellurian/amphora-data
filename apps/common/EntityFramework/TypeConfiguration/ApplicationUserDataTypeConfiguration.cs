using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration
{
    public class ApplicationUserDataTypeConfiguration : IEntityTypeConfiguration<ApplicationUserDataModel>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserDataModel> builder)
        {
            builder.OwnsOne(_ => _.ContactInformation);

            builder.HasOne(p => p.Organisation)
                .WithMany()
                .HasForeignKey(p => p.OrganisationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.OwnsOne(_ => _.PinnedAmphorae, p =>
            {
                p.HasOne(_ => _.Amphora1).WithMany().HasForeignKey(_ => _.AmphoraId1);
                p.HasOne(_ => _.Amphora2).WithMany().HasForeignKey(_ => _.AmphoraId2);
                p.HasOne(_ => _.Amphora3).WithMany().HasForeignKey(_ => _.AmphoraId3);
                p.HasOne(_ => _.Amphora4).WithMany().HasForeignKey(_ => _.AmphoraId4);
                p.HasOne(_ => _.Amphora5).WithMany().HasForeignKey(_ => _.AmphoraId5);
                p.HasOne(_ => _.Amphora6).WithMany().HasForeignKey(_ => _.AmphoraId6);
            });
        }
    }
}