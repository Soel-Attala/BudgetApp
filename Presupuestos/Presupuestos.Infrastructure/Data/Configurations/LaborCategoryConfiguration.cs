using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Presupuestos.Domain.Models;

namespace Presupuestos.Infrastructure.Data.Configurations;

public class LaborCategoryConfiguration : IEntityTypeConfiguration<LaborCategory>
{
    public void Configure(EntityTypeBuilder<LaborCategory> b)
    {
        b.ToTable("LaborCategories");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
    }
}
