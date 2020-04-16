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
        }
    }
}