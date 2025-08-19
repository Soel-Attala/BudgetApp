using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Models;
using System.Collections.ObjectModel;

namespace Presupuestos.WPF.ViewModels;

public partial class LaborViewModel : ObservableObject
{
    private readonly ILaborService _svc;

    public ObservableCollection<LaborItem> Items { get; } = new();
    public ObservableCollection<LaborCategory> Categories { get; } = new();

    [ObservableProperty] private LaborItem? _selectedItem;
    [ObservableProperty] private LaborCategory? _selectedCategory;
    [ObservableProperty] private string? _searchText;

    public LaborViewModel(ILaborService svc) { _svc = svc; _ = LoadAsync(); }

    private async Task LoadAsync()
    {
        Categories.Clear();
        foreach (var c in await _svc.GetCategoriesAsync()) Categories.Add(c);

        Items.Clear();
        foreach (var i in await _svc.GetAllAsync()) Items.Add(i);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        Items.Clear();
        foreach (var i in await _svc.SearchAsync(SearchText, SelectedCategory?.Id)) Items.Add(i);
    }

    [RelayCommand]
    private async Task NewCategoryAsync()
    {
        var c = new LaborCategory { Name = "Nueva categoría" };
        await _svc.AddCategoryAsync(c);
        Categories.Add(c);
        SelectedCategory = c;
    }

    [RelayCommand]
    private void NewItem()
    {
        var it = new LaborItem { Name = "Nuevo ítem", Unit = "hora", PriceARS = 0m, CategoryId = SelectedCategory?.Id, Category = SelectedCategory };
        Items.Add(SelectedItem = it);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        foreach (var it in Items.Where(i => i.Id == Guid.Empty)) { it.Id = Guid.NewGuid(); await _svc.AddAsync(it); }
        foreach (var it in Items.Where(i => i.Id != Guid.Empty)) await _svc.UpdateAsync(it);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        await _svc.DeleteAsync(SelectedItem.Id);
        Items.Remove(SelectedItem);
        SelectedItem = null;
    }
}
