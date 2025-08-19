using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Presupuestos.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _sp;

    [ObservableProperty] private ObservableObject? _currentViewModel;

    public MainViewModel(IServiceProvider sp)
    {
        _sp = sp;
        CurrentViewModel = _sp.GetRequiredService<BudgetWizardViewModel>();
    }

    [RelayCommand] private void GoWizard() => CurrentViewModel = _sp.GetRequiredService<BudgetWizardViewModel>();
    [RelayCommand] private void GoMaterials() => CurrentViewModel = _sp.GetRequiredService<MaterialViewModel>();
    [RelayCommand] private void GoLabor() => CurrentViewModel = _sp.GetRequiredService<LaborViewModel>();
    [RelayCommand] private void GoSubgroups() => CurrentViewModel = _sp.GetRequiredService<SubgroupViewModel>();
    [RelayCommand] private void GoSettings() => CurrentViewModel = _sp.GetRequiredService<SettingsViewModel>();
}
