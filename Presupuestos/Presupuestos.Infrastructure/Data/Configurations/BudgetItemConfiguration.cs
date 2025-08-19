using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Presupuestos.Domain.Enums;
using Presupuestos.Domain.Models;

namespace Presupuestos.Infrastructure.Data.Configurations;

public class BudgetItemConfiguration : IEntityTypeConfiguration<BudgetItem>
{
    public void Configure(EntityTypeBuilder<BudgetItem> b)
    {
        b.ToTable("BudgetItems");
        b.HasKey(x => x.Id);

        b.Property<BudgetItemType>(x => x.Type).IsRequired();

        // Materiales
        b.Property(x => x.MaterialId);
        b.Property(x => x.MaterialName).HasMaxLength(300);
        b.Property(x => x.PriceUSD).HasColumnType("decimal(18,6)");
        b.Property(x => x.Quantity).HasColumnType("decimal(18,6)");

        // Subgrupo
        b.Property(x => x.SubgroupId);
        b.Property(x => x.SubgroupName).HasMaxLength(300);

        // Mano de obra
        b.Property(x => x.LaborItemId);
        b.Property(x => x.LaborItemName).HasMaxLength(300);
        b.Property(x => x.PriceARS).HasColumnType("decimal(18,6)");
        b.Property(x => x.LaborQuantity).HasColumnType("decimal(18,6)");

        // FK a Budget (shadow)
        b.Property<Guid>("BudgetId");
        b.HasIndex("BudgetId");
        b.HasIndex(x => x.Type);
    }
}
