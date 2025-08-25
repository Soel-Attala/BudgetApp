using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Infrastructure.Data;
using Presupuestos.Infrastructure.Services;
using Presupuestos.Reporting;
using Presupuestos.WPF.ViewModels;
using Presupuestos.WPF.Views;

namespace Presupuestos.WPF;

public partial class App : System.Windows.Application
{
    public static ServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Config
        var cfg = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .Build();

        // DI
        var sc = new ServiceCollection();

        // DB en %LOCALAPPDATA%\Presupuestos\presupuestos.db
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbDir = Path.Combine(appData, "Presupuestos");
        Directory.CreateDirectory(dbDir);
        var dbPath = Path.Combine(dbDir, "presupuestos.db");

        sc.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

        // HttpClient
        sc.AddHttpClient();

        // Services (Application -> Infrastructure)
        sc.AddScoped<IMaterialService, MaterialService>();
        sc.AddScoped<ILaborService, LaborService>();
        sc.AddScoped<ISubgroupService, SubgroupService>();
        sc.AddScoped<IBudgetService, BudgetService>();
        sc.AddScoped<IExchangeRateService, ExchangeRateService>();

        // Reporting
        sc.AddScoped<IBudgetReportService, BudgetPdfGenerator>();

        // ViewModels
        sc.AddTransient<MainViewModel>();
        sc.AddTransient<BudgetWizardViewModel>();
        sc.AddTransient<MaterialViewModel>();
        sc.AddTransient<LaborViewModel>();
        sc.AddTransient<SubgroupViewModel>();
        sc.AddTransient<SettingsViewModel>();

        // Views
        sc.AddTransient<MainWindow>();
        sc.AddTransient<BudgetWizardView>();
        sc.AddTransient<MaterialView>();
        sc.AddTransient<LaborView>();
        sc.AddTransient<SubgroupView>();
        sc.AddTransient<SettingsView>();

        // Pickers
        sc.AddTransient<MaterialPickerDialog>();
        sc.AddTransient<LaborPickerDialog>();

        Services = sc.BuildServiceProvider();

        // Migrar
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        // Lanzar
        var window = Services.GetRequiredService<MainWindow>();
        window.DataContext = Services.GetRequiredService<MainViewModel>();
        window.Show();
    }
}

