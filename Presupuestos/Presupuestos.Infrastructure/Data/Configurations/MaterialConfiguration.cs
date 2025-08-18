using Microsoft.EntityFrameworkCore;
using Presupuestos.Domain.Models;
using Presupuestos.Infrastructure.Data.Configurations;

namespace Presupuestos.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Catálogos
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<LaborCategory> LaborCategories => Set<LaborCategory>();
    public DbSet<LaborItem> LaborItems => Set<LaborItem>();

    // Subgrupos
    public DbSet<Subgroup> Subgroups => Set<Subgroup>();
    public DbSet<SubgroupMaterial> SubgroupMaterials => Set<SubgroupMaterial>();

    // Presupuestos
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
    // BudgetTaxes se mapea como Owned de Budget (misma tabla)

    // FX
    public DbSet<ExchangeRateRecord> ExchangeRates => Set<ExchangeRateRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configs por tipo
        modelBuilder.ApplyConfiguration(new MaterialConfiguration());
        modelBuilder.ApplyConfiguration(new LaborCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new LaborItemConfiguration());

        modelBuilder.ApplyConfiguration(new SubgroupConfiguration());
        modelBuilder.ApplyConfiguration(new SubgroupMaterialConfiguration());

        modelBuilder.ApplyConfiguration(new BudgetConfiguration());
        modelBuilder.ApplyConfiguration(new BudgetItemConfiguration());

        modelBuilder.ApplyConfiguration(new ExchangeRateRecordConfiguration());
    }
}
