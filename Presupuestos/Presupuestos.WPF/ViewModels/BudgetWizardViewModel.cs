using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Enums;
using Presupuestos.Domain.Models;
using Presupuestos.WPF.Views;
using System.Collections.ObjectModel;
using System.IO;

using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Presupuestos.WPF.ViewModels;

public partial class BudgetWizardViewModel : ObservableObject
{
    private readonly IMaterialService _materialService;
    private readonly ILaborService _laborService;
    private readonly ISubgroupService _subgroupService;
    private readonly IBudgetService _budgetService;
    private readonly IBudgetReportService _report;
    private readonly IExchangeRateService _fx;
    private readonly IServiceProvider _sp;

    // Campos de presupuesto
    [ObservableProperty] private string _name = "Nuevo Presupuesto";
    [ObservableProperty] private DateTime _date = DateTime.Now;

    [ObservableProperty] private string? _clientName;
    [ObservableProperty] private string? _clientEmail;
    [ObservableProperty] private string? _clientPhone;
    [ObservableProperty] private string? _clientAddress;

    [ObservableProperty] private decimal _dollarRate = 1m;
    [ObservableProperty] private decimal _benefitPercentage = 0m;
    [ObservableProperty] private bool _includeFees = false;
    [ObservableProperty] private decimal _feesCost = 0m;
    [ObservableProperty] private bool _includeLabor = false;
    [ObservableProperty] private decimal _laborCost = 0m;

    [ObservableProperty] private BudgetTaxes _taxes = new();

    public ObservableCollection<BudgetItem> Items { get; } = new();

    public BudgetWizardViewModel(
      IMaterialService materialService,
      ILaborService laborService,
      ISubgroupService subgroupService,
      IBudgetService budgetService,
      IBudgetReportService report,
      IExchangeRateService fx,
      IServiceProvider sp)
    {
        _materialService = materialService;
        _laborService = laborService;
        _subgroupService = subgroupService;
        _budgetService = budgetService;
        _report = report;
        _fx = fx;
        _sp = sp;
    }

    private Budget BuildBudget() => new()
    {
        Name = Name,
        Date = Date,
        ClientName = ClientName?.Trim(),
        ClientEmail = ClientEmail?.Trim(),
        ClientPhone = ClientPhone?.Trim(),
        ClientAddress = ClientAddress?.Trim(),
        DollarRate = DollarRate,
        BenefitPercentage = BenefitPercentage,
        IncludeFees = IncludeFees,
        FeesCost = FeesCost,
        IncludeLabor = IncludeLabor,
        LaborCost = LaborCost,
        Taxes = new BudgetTaxes
        {
            IncludeTaxes = Taxes.IncludeTaxes,
            IVA = Taxes.IVA,
            IVAT = Taxes.IVAT,
            IB = Taxes.IB,
            IG = Taxes.IG,
            IC = Taxes.IC,
            BankFees = Taxes.BankFees
        },
        Items = Items.ToList()
    };

    [RelayCommand]
    private async Task RefreshDollarAsync()
    {
        var rec = await _fx.RefreshUsdArsAsync();
        if (rec != null) DollarRate = rec.Rate;
    }

    [RelayCommand]
    private async Task OpenMaterialPickerAsync()
    {
        var dlg = _sp.GetRequiredService<MaterialPickerDialog>();
        dlg.Owner = System.Windows.Application.Current?.MainWindow;
        dlg.SetCatalog(await _materialService.GetAllAsync());
        if (dlg.ShowDialog() == true)
        {
            foreach (var sel in dlg.Result)
            {
                Items.Add(new BudgetItem
                {
                    Type = BudgetItemType.Material,
                    MaterialId = sel.Material.Id,
                    MaterialName = sel.Material.Name,
                    PriceUSD = sel.Material.PriceUSD,
                    Quantity = sel.Quantity
                });
            }
        }
    }

    [RelayCommand]
    private async Task OpenLaborPickerAsync()
    {
        var dlg = _sp.GetRequiredService<LaborPickerDialog>();
        dlg.Owner = System.Windows.Application.Current?.MainWindow;
        var items = await _laborService.GetAllAsync();
        var cats = await _laborService.GetCategoriesAsync();
        dlg.SetCatalog(items, cats);
        if (dlg.ShowDialog() == true)
        {
            foreach (var sel in dlg.Result)
            {
                Items.Add(new BudgetItem
                {
                    Type = BudgetItemType.LaborItem,
                    LaborItemId = sel.Item.Id,
                    LaborItemName = sel.Item.Name,
                    PriceARS = sel.Item.PriceARS,
                    LaborQuantity = sel.Quantity
                });
            }
        }
    }

    [RelayCommand]
    private async Task ImportSubgroupAsync()
    {
        // simple: toma el primero por ejemplo; adaptá a tu selector si lo tenés
        var all = await _subgroupService.GetAllAsync();
        var sg = all.FirstOrDefault();
        if (sg is null) return;

        Items.Add(new BudgetItem { Type = BudgetItemType.SubgroupHeader, SubgroupId = sg.Id, SubgroupName = sg.Name });
        foreach (var m in sg.Materials)
        {
            Items.Add(new BudgetItem
            {
                Type = BudgetItemType.SubgroupMaterial,
                SubgroupId = sg.Id,
                SubgroupName = sg.Name,
                MaterialId = m.MaterialId,
                MaterialName = m.MaterialName,
                PriceUSD = m.PriceUSD,
                Quantity = m.Quantity
            });
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var b = BuildBudget();
        await _budgetService.AddAsync(b);
    }

    [RelayCommand]
    private void ExportPdf()
    {
        var bytes = _report.Generate(BuildBudget());
        var dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var safe = string.Concat(Name.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
        if (string.IsNullOrWhiteSpace(safe)) safe = "Presupuesto";
        var path = Path.Combine(dir, $"{safe}_{Date:yyyyMMdd}.pdf");
        File.WriteAllBytes(path, bytes);
    }
}
