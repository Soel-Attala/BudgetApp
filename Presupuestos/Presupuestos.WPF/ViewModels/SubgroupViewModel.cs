using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Models;
using Presupuestos.WPF.Views;
using System.Collections.ObjectModel;

namespace Presupuestos.WPF.ViewModels;

public partial class SubgroupViewModel : ObservableObject
{
    private readonly ISubgroupService _svc;
    private readonly IMaterialService _materialsSvc;
    private readonly IServiceProvider _sp;

    public ObservableCollection<Subgroup> Subgroups { get; } = new();
    public ObservableCollection<SubgroupMaterial> Materials { get; } = new();

    [ObservableProperty] private Subgroup? _selected;
    partial void OnSelectedChanged(Subgroup? value)
    {
        Materials.Clear();
        if (value != null && value.Materials != null)
            foreach (var m in value.Materials.OrderBy(x => x.MaterialName))
                Materials.Add(m);
    }

    [ObservableProperty] private string? _searchText;

    public SubgroupViewModel(ISubgroupService svc, IMaterialService materialsSvc, IServiceProvider sp)
    {
        _svc = svc;
        _materialsSvc = materialsSvc;
        _sp = sp;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        Subgroups.Clear();
        foreach (var sg in await _svc.GetAllAsync())
            Subgroups.Add(sg);

        Selected = Subgroups.FirstOrDefault();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        Subgroups.Clear();
        foreach (var sg in await _svc.SearchAsync(SearchText))
            Subgroups.Add(sg);

        Selected = Subgroups.FirstOrDefault();
    }

    [RelayCommand]
    private void New()
    {
        var sg = new Subgroup { Name = "Nuevo subgrupo" };
        Subgroups.Add(sg);
        Selected = sg;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Selected is null) return;

        // Sincronizamos la colección Materials con Selected.Materials
        Selected.Materials = Materials.ToList();

        if (Selected.Id == Guid.Empty)
            Selected.Id = Guid.NewGuid();

        await _svc.UpdateAsync(Selected);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Selected is null) return;
        if (Selected.Id != Guid.Empty)
            await _svc.DeleteAsync(Selected.Id);

        Subgroups.Remove(Selected);
        Selected = Subgroups.FirstOrDefault();
    }

    [RelayCommand]
    private async Task AddMaterialsAsync()
    {
        // Abrimos el picker y agregamos al subgrupo los seleccionados
        var dlg = _sp.GetRequiredService<MaterialPickerDialog>();
        dlg.Owner = System.Windows.Application.Current?.MainWindow;
        dlg.SetCatalog(await _materialsSvc.GetAllAsync());

        if (dlg.ShowDialog() == true)
        {
            foreach (var sel in dlg.Result)
            {
                Materials.Add(new SubgroupMaterial
                {
                    Id = Guid.NewGuid(),
                    SubgroupId = Selected?.Id ?? Guid.Empty,
                    MaterialId = sel.Material.Id,
                    MaterialName = sel.Material.Name,
                    PriceUSD = sel.Material.PriceUSD,
                    Quantity = sel.Quantity
                });
            }
        }
    }
}
