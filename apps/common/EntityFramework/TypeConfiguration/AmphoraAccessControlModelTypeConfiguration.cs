using Amphora.Common.Models.Amphorae;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration
{
    public class AmphoraAccessControlModelTypeConfiguration : IEntityTypeConfiguration<AmphoraAccessControlModel>
    {
        public void Configure(EntityTypeBuilder<AmphoraAccessControlModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.OwnsMany(_ => _.OrganisationAccessRules, _ =>
            {
                _.Property(r => r.Id).ValueGeneratedOnAdd();
                _.HasOne(r => r.Organisation)
                    .WithMany()
                    .HasForeignKey(r => r.OrganisationId);
            });

            builder.OwnsMany(_ => _.UserAccessRules, _ =>
            {
                _.Property(r => r.Id).ValueGeneratedOnAdd();
                _.HasOne(r => r.UserData)
                    .WithMany()
                    .HasForeignKey(r => r.UserDataId);
            });

            builder.HasOne(_ => _.AllRule);
        }
    }
}