using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presupuestos.Application.Interfaces;
using Presupuestos.Infrastructure.Data;
using Presupuestos.Infrastructure.Services;
using Presupuestos.Reporting;
using Presupuestos.WPF.ViewModels;
using Presupuestos.WPF.Views;
using System;
using System.IO;
using System.Windows;

namespace Presupuestos.WPF;

public partial class App : Application
{
    public static ServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1) Configuration (appsettings.json opcional)
        var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var config = builder.Build();

        // 2) DI
        var services = new ServiceCollection();

        // ---- DbPath (SQLite local en AppData) ----
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbDir = Path.Combine(appData, "Presupuestos");
        Directory.CreateDirectory(dbDir);
        var dbPath = Path.Combine(dbDir, "presupuestos.db");

        services.AddDbContext<AppDbContext>(opt =>
          opt.UseSqlite($"Data Source={dbPath}"));

        // ---- HttpClient ----
        services.AddHttpClient();

        // ---- Servicios de Aplicación (Interfaces -> Implementaciones de Infrastructure/Reporting) ----
        services.AddScoped<IMaterialService, MaterialService>();
        services.AddScoped<ILaborService, LaborService>();
        services.AddScoped<ISubgroupService, SubgroupService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IExchangeRateService, ExchangeRateService>();

        // Reporting (QuestPDF)
        services.AddScoped<IBudgetReportService, BudgetPdfGenerator>();

        // ---- ViewModels ----
        services.AddTransient<MainViewModel>();
        services.AddTransient<BudgetWizardViewModel>();
        services.AddTransient<MaterialViewModel>();
        services.AddTransient<LaborViewModel>();
        services.AddTransient<SubgroupViewModel>();
        services.AddTransient<SettingsViewModel>();

        // ---- Views (Ventanas/Dialogs) ----
        services.AddTransient<MainWindow>();
        services.AddTransient<BudgetWizardView>();
        services.AddTransient<MaterialView>();
        services.AddTransient<LaborView>();
        services.AddTransient<SubgroupView>();
        services.AddTransient<SettingsView>();

        // Pickers / Diálogos
        services.AddTransient<MaterialPickerDialog>();
        services.AddTransient<LaborPickerDialog>();

        Services = services.BuildServiceProvider();

        // 3) Migrar/crear DB si falta
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        // 4) Mostrar MainWindow con VM por DI
        var main = Services.GetRequiredService<MainWindow>();
        // Si tu MainWindow usa DataContext por DI:
        main.DataContext = Services.GetRequiredService<MainViewModel>();
        main.Show();
    }
}
