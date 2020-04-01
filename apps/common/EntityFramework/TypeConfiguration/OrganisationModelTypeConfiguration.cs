using Amphora.Common.Models.Organisations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration
{
    public class OrganisationModelTypeConfiguration : IEntityTypeConfiguration<OrganisationModel>
    {
        public void Configure(EntityTypeBuilder<OrganisationModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            // new global invitations
            builder.HasMany(_ => _.GlobalInvitations)
                .WithOne(_ => _.TargetOrganisation!)
                .HasForeignKey(_ => _.TargetOrganisationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.OwnsMany(b => b.Memberships, membership =>
            {
                membership.HasKey(nameof(Membership.UserId));
                membership.HasOne(b => b.User).WithMany().HasForeignKey(u => u.UserId);
            });
            builder.HasOne(_ => _.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);
            builder.OwnsMany(_ => _.TermsAndConditions, termsAndConditions =>
            {
                termsAndConditions.WithOwner(_ => _.Organisation).HasForeignKey(_ => _.OrganisationId);
                termsAndConditions.HasKey(_ => _.Id);
            });
            builder.OwnsMany(p => p.TermsAndConditionsAccepted, a =>
            {
                a.WithOwner(b => b.AcceptedByOrganisation).HasForeignKey(_ => _.AcceptedByOrganisationId);
                a.HasOne(b => b.TermsAndConditionsOrganisation)
                    .WithMany()
                    .HasForeignKey(b => b.TermsAndConditionsOrganisationId)
                    .OnDelete(DeleteBehavior.NoAction);

                a.HasKey(_ => new { _.TermsAndConditionsId, _.TermsAndConditionsOrganisationId }); // dual key
            });
            // Pinned Amphorae
            builder.OwnsOne(_ => _.PinnedAmphorae, p =>
            {
                p.HasOne(_ => _!.Amphora1).WithMany().HasForeignKey(_ => _!.AmphoraId1);
                p.HasOne(_ => _!.Amphora2).WithMany().HasForeignKey(_ => _!.AmphoraId2);
                p.HasOne(_ => _!.Amphora3).WithMany().HasForeignKey(_ => _!.AmphoraId3);
                p.HasOne(_ => _!.Amphora4).WithMany().HasForeignKey(_ => _!.AmphoraId4);
                p.HasOne(_ => _!.Amphora5).WithMany().HasForeignKey(_ => _!.AmphoraId5);
                p.HasOne(_ => _!.Amphora6).WithMany().HasForeignKey(_ => _!.AmphoraId6);
            });

            // configuration
            builder.OwnsOne(_ => _.Configuration, configuration =>
            {
                // nothing specific
            });

            // cache
            builder.Property(_ => _.Cache).HasJsonConversion();

            // Account
            builder.OwnsOne(_ => _.Account, account =>
            {
                account.WithOwner(_ => _!.Organisation).HasForeignKey(_ => _!.OrganisationId);
                account.OwnsOne(_ => _!.Plan, plan =>
                {
                    // nothing specific
                });
                account.OwnsMany(_ => _!.Credits, credit =>
                {
                    credit.Property(_ => _.Id).ValueGeneratedOnAdd();
                    credit.HasKey(_ => _.Id);
                    credit.WithOwner(_ => _.Account);
                });
                account.OwnsMany(_ => _!.Debits, debit =>
                {
                    debit.Property(_ => _.Id).ValueGeneratedOnAdd();
                    debit.HasKey(_ => _.Id);
                    debit.WithOwner(_ => _.Account);
                });
                account.OwnsMany(b => b!.Invoices, invoice =>
                {
                    invoice.Property(_ => _.Id).ValueGeneratedOnAdd();
                    invoice.OwnsMany(_ => _.Transactions, transaction =>
                    {
                        transaction.Property(_ => _.Id).ValueGeneratedOnAdd();
                        transaction.HasKey(_ => _.Id);
                    });
                    invoice.HasKey(_ => _.Id);
                    invoice.WithOwner(_ => _.Account);
                });
            });
            // builder.HasMany(_ => _.Purchases).WithOne(p => p.PurchasedByOrganisation).HasForeignKey(_ => _.PurchasedByOrganisationId);
        }
    }
}