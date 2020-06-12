using Amphora.Common.Models.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration.AmphoraContext
{
    public class InvitationModelTypeConfiguration : IEntityTypeConfiguration<InvitationModel>
    {
        public void Configure(EntityTypeBuilder<InvitationModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder
                .HasOne(_ => _.TargetOrganisation)
                .WithMany(_ => _!.GlobalInvitations)
                .HasForeignKey(_ => _.TargetOrganisationId);
        }
    }
}