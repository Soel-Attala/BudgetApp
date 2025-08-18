using System.ComponentModel;
using System.Runtime.CompilerServices;
using Presupuestos.Domain.Enums;

namespace Presupuestos.Domain.Models;

public class BudgetItem : INotifyPropertyChanged
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private BudgetItemType _type;
    public BudgetItemType Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(); }
    }

    // -------- Materiales (USD) --------
    public Guid? MaterialId { get; set; }

    private string? _materialName;
    public string? MaterialName
    {
        get => _materialName;
        set { _materialName = value; OnPropertyChanged(); }
    }

    private decimal? _priceUSD;
    public decimal? PriceUSD
    {
        get => _priceUSD;
        set { _priceUSD = value; OnPropertyChanged(); }
    }

    private decimal? _quantity;
    public decimal? Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); }
    }

    // -------- Subgrupo --------
    public Guid? SubgroupId { get; set; }

    private string? _subgroupName;
    public string? SubgroupName
    {
        get => _subgroupName;
        set { _subgroupName = value; OnPropertyChanged(); }
    }

    // -------- Mano de obra (ARS) --------
    public Guid? LaborItemId { get; set; }

    private string? _laborItemName;
    public string? LaborItemName
    {
        get => _laborItemName;
        set { _laborItemName = value; OnPropertyChanged(); }
    }

    private decimal? _priceARS;
    public decimal? PriceARS
    {
        get => _priceARS;
        set { _priceARS = value; OnPropertyChanged(); }
    }

    private decimal? _laborQuantity;
    public decimal? LaborQuantity
    {
        get => _laborQuantity;
        set { _laborQuantity = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
