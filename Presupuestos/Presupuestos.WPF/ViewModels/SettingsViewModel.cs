using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Models;
using System.Collections.ObjectModel;

namespace Presupuestos.WPF.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IExchangeRateService _fx;

    public ObservableCollection<ExchangeRateRecord> Recent { get; } = new();

    public SettingsViewModel(IExchangeRateService fx)
    {
        _fx = fx;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        Recent.Clear();
        foreach (var r in await _fx.GetRecentAsync(20))
            Recent.Add(r);
    }

    [RelayCommand]
    private async Task RefreshFromExHostAsync()
    {
        await _fx.RefreshUsdArsAsync("exhost");
        await LoadAsync();
    }

    [RelayCommand]
    private async Task RefreshDolarsiOficialAsync()
    {
        await _fx.RefreshUsdArsAsync("dolarsi:oficial");
        await LoadAsync();
    }

    [RelayCommand]
    private async Task RefreshDolarsiBlueAsync()
    {
        await _fx.RefreshUsdArsAsync("dolarsi:blue");
        await LoadAsync();
    }
}
