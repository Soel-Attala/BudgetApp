using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Presupuestos.Domain.Models;

namespace Presupuestos.Infrastructure.Data.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> b)
    {
        b.ToTable("Budgets");
        b.HasKey(x => x.Id);

        // Identificación
        b.Property(x => x.Name).HasMaxLength(300).IsRequired();

        // Cliente
        b.Property(x => x.ClientName).HasMaxLength(300);
        b.Property(x => x.ClientEmail).HasMaxLength(300);
        b.Property(x => x.ClientPhone).HasMaxLength(100);
        b.Property(x => x.ClientAddress).HasMaxLength(500);

        // Económicos
        b.Property(x => x.DollarRate).HasColumnType("decimal(18,6)");
        b.Property(x => x.BenefitPercentage).HasColumnType("decimal(9,4)");
        b.Property(x => x.FeesCost).HasColumnType("decimal(18,6)");
        b.Property(x => x.LaborCost).HasColumnType("decimal(18,6)");

        // Owned: Taxes (mismo row que Budget)
        b.OwnsOne(x => x.Taxes, taxes =>
        {
            // Ignoramos el Id de BudgetTaxes para no mapear columna innecesaria
            taxes.Ignore(t => t.Id);

            taxes.Property(t => t.IncludeTaxes);
            taxes.Property(t => t.IVA).HasColumnType("decimal(9,4)");
            taxes.Property(t => t.IVAT).HasColumnType("decimal(9,4)");
            taxes.Property(t => t.IB).HasColumnType("decimal(9,4)");
            taxes.Property(t => t.IG).HasColumnType("decimal(9,4)");
            taxes.Property(t => t.IC).HasColumnType("decimal(9,4)");
            taxes.Property(t => t.BankFees).HasColumnType("decimal(9,4)");

            // No mapeamos TotalRateFraction (es calculado)
            taxes.Ignore(t => t.TotalRateFraction);

            taxes.ToTable("Budgets"); // mismos campos en la tabla Budgets
        });

        // Relación con Items
        b.HasMany<BudgetItem>("Items")
         .WithOne()
         .HasForeignKey("BudgetId")
         .OnDelete(DeleteBehavior.Cascade);

        // Índices útiles
        b.HasIndex(x => x.Date);
        b.HasIndex(x => x.Name);
    }
}
