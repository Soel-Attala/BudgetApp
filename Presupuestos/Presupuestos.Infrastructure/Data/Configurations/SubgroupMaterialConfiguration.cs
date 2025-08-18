using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Presupuestos.Domain.Models;

namespace Presupuestos.Infrastructure.Data.Configurations;

public class SubgroupMaterialConfiguration : IEntityTypeConfiguration<SubgroupMaterial>
{
    public void Configure(EntityTypeBuilder<SubgroupMaterial> b)
    {
        b.ToTable("SubgroupMaterials");
        b.HasKey(x => x.Id);

        b.Property(x => x.MaterialName).HasMaxLength(300).IsRequired();
        b.Property(x => x.PriceUSD).HasColumnType("decimal(18,6)");
        b.Property(x => x.Quantity).HasColumnType("decimal(18,6)");
    }
}
