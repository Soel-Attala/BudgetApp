using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Presupuestos.Domain.Models;

namespace Presupuestos.Infrastructure.Data.Configurations;

public class ExchangeRateRecordConfiguration : IEntityTypeConfiguration<ExchangeRateRecord>
{
    public void Configure(EntityTypeBuilder<ExchangeRateRecord> b)
    {
        b.ToTable("ExchangeRates");
        b.HasKey(x => x.Id);

        b.Property(x => x.BaseCurrency).HasMaxLength(10).IsRequired();
        b.Property(x => x.QuoteCurrency).HasMaxLength(10).IsRequired();
        b.Property(x => x.Rate).HasColumnType("decimal(18,8)"); // más precisión
        b.Property(x => x.Provider).HasMaxLength(100);
        b.Property(x => x.Note).HasMaxLength(300);

        b.HasIndex(x => x.TimestampUtc);
        b.HasIndex(x => new { x.BaseCurrency, x.QuoteCurrency });
    }
}
