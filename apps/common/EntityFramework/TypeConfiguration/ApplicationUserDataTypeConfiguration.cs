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
        }
    }
}