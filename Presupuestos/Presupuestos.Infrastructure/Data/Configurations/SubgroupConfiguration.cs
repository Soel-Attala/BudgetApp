using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Presupuestos.Domain.Models;

namespace Presupuestos.Infrastructure.Data.Configurations;

public class SubgroupConfiguration : IEntityTypeConfiguration<Subgroup>
{
    public void Configure(EntityTypeBuilder<Subgroup> b)
    {
        b.ToTable("Subgroups");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(300).IsRequired();

        b.HasMany(sg => sg.Materials)
         .WithOne()
         .HasForeignKey(sgm => sgm.SubgroupId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
