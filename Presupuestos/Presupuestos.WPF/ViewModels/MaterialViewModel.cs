using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace Presupuestos.WPF.ViewModels;

public partial class MaterialViewModel : ObservableObject
{
    private readonly IMaterialService _svc;
    public ObservableCollection<Material> Items { get; } = new();

    [ObservableProperty] private Material? _selected;
    [ObservableProperty] private string? _searchText;

    public MaterialViewModel(IMaterialService svc) { _svc = svc; _ = LoadAsync(); }

    private async Task LoadAsync()
    {
        Items.Clear();
        foreach (var m in await _svc.GetAllAsync()) Items.Add(m);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        Items.Clear();
        foreach (var m in await _svc.SearchAsync(SearchText)) Items.Add(m);
    }

    [RelayCommand] private void New() => Items.Add(Selected = new Material { Name = "Nuevo", PriceUSD = 0m });
    [RelayCommand] private async Task SaveAsync() { foreach (var m in Items) if (m.Id == Guid.Empty) m.Id = Guid.NewGuid(); foreach (var m in Items) await _svc.UpdateAsync(m); }
    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Selected is null) return;
        await _svc.DeleteAsync(Selected.Id);
        Items.Remove(Selected);
        Selected = null;
    }
}
