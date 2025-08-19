using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Presupuestos.Domain.Models;

namespace Presupuestos.Infrastructure.Data.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> b)
    {
        b.ToTable("Materials");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(300).IsRequired();
        b.Property(x => x.PriceUSD).HasColumnType("decimal(18,6)"); // preciso, sin redondeo
    }
}
