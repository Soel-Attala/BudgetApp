using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace Presupuestos.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        var dbDir = Path.Combine(appData, "Presupuestos");
        Directory.CreateDirectory(dbDir);
        var dbPath = Path.Combine(dbDir, "presupuestos.db");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        return new AppDbContext(optionsBuilder.Options);
    }
}
