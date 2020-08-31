using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration.AmphoraContext
{
    public class InvoiceModelTypeConfiguration : IEntityTypeConfiguration<InvoiceModel>
    {
        public void Configure(EntityTypeBuilder<InvoiceModel> builder)
        {
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            builder.HasOne(_ => _.Organisation).WithMany(_ => _.Invoices).HasForeignKey(_ => _.OrganisationId);

            builder.OwnsMany(_ => _.Transactions, transaction =>
            {
                transaction.Property(_ => _.Id).ValueGeneratedOnAdd();
                transaction.HasKey(_ => _.Id);
            });
        }
    }
}