using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Presupuestos.Domain.Models;

namespace Presupuestos.Infrastructure.Data.Configurations;

public class LaborItemConfiguration : IEntityTypeConfiguration<LaborItem>
{
    public void Configure(EntityTypeBuilder<LaborItem> b)
    {
        b.ToTable("LaborItems");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(300).IsRequired();
        b.Property(x => x.Unit).HasMaxLength(50);
        b.Property(x => x.PriceARS).HasColumnType("decimal(18,6)");

        b.HasOne(li => li.Category)
          .WithMany(c => c.Items)
          .HasForeignKey(li => li.CategoryId)
          .OnDelete(DeleteBehavior.SetNull);
    }
}
