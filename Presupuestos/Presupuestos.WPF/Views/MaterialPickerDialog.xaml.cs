using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Presupuestos.Domain.Models;

namespace Presupuestos.WPF.Views;

public partial class MaterialPickerDialog : Window
{
    public class Row : INotifyPropertyChanged
    {
        public Material Material { get; set; } = null!;
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; PropertyChanged?.Invoke(this, new(nameof(IsSelected))); } }
        private decimal _quantity = 1m;
        public decimal Quantity { get => _quantity; set { _quantity = value < 0 ? 0 : value; PropertyChanged?.Invoke(this, new(nameof(Quantity))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class Selection { public Material Material { get; set; } = null!; public decimal Quantity { get; set; } }

    public ObservableCollection<Row> Rows { get; } = new();
    private ICollectionView _view;
    public List<Selection> Result { get; private set; } = new();

    public MaterialPickerDialog()
    {
        InitializeComponent();
        Grid.ItemsSource = Rows;
        _view = CollectionViewSource.GetDefaultView(Rows);
        _view.Filter = Filter;
    }

    public void SetCatalog(IEnumerable<Material> materials)
    {
        Rows.Clear();
        foreach (var m in materials.OrderBy(x => x.Name))
            Rows.Add(new Row { Material = m, IsSelected = false, Quantity = 1m });
        _view.Refresh();
    }

    private bool Filter(object obj)
    {
        if (obj is not Row r) return false;
        var q = (SearchBox.Text ?? "").Trim().ToLowerInvariant();
        if (OnlySelectedCheck.IsChecked == true && !r.IsSelected) return false;
        if (!string.IsNullOrEmpty(q))
            return (r.Material.Name ?? "").ToLowerInvariant().Contains(q);
        return true;
    }

    private void SearchBox_TextChanged(object s, System.Windows.Controls.TextChangedEventArgs e) => _view.Refresh();
    private void OnlySelectedCheck_Changed(object s, RoutedEventArgs e) => _view.Refresh();
    private void Clear_Click(object s, RoutedEventArgs e) { SearchBox.Text = ""; OnlySelectedCheck.IsChecked = false; _view.Refresh(); }
    private void SelectAll_Click(object s, RoutedEventArgs e) { foreach (var r in Rows) r.IsSelected = true; }
    private void ClearSelection_Click(object s, RoutedEventArgs e) { foreach (var r in Rows) r.IsSelected = false; }
    private void Cancel_Click(object s, RoutedEventArgs e) { DialogResult = false; Close(); }
    private void Accept_Click(object s, RoutedEventArgs e)
    {
        Result = Rows.Where(r => r.IsSelected && r.Quantity > 0m)
                     .Select(r => new Selection { Material = r.Material, Quantity = r.Quantity })
                     .ToList();
        DialogResult = true; Close();
    }
}
