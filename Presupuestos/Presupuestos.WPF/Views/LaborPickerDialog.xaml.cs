using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Presupuestos.Domain.Models;

namespace Presupuestos.WPF.Views;

public partial class LaborPickerDialog : Window
{
    public class Row : INotifyPropertyChanged
    {
        public LaborItem Item { get; set; } = null!;
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; PropertyChanged?.Invoke(this, new(nameof(IsSelected))); } }
        private decimal _quantity = 1m;
        public decimal Quantity { get => _quantity; set { _quantity = value < 0 ? 0 : value; PropertyChanged?.Invoke(this, new(nameof(Quantity))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class Selection { public LaborItem Item { get; set; } = null!; public decimal Quantity { get; set; } }

    public ObservableCollection<Row> Rows { get; } = new();
    private ICollectionView _view;
    public List<Selection> Result { get; private set; } = new();
    private List<LaborCategory> _categories = new();

    public LaborPickerDialog()
    {
        InitializeComponent();
        Grid.ItemsSource = Rows;
        _view = CollectionViewSource.GetDefaultView(Rows);
        _view.Filter = Filter;
    }

    public void SetCatalog(IEnumerable<LaborItem> items, IEnumerable<LaborCategory> categories)
    {
        _categories = categories.OrderBy(c => c.Name).ToList();
        CategoryCombo.ItemsSource = _categories;
        CategoryCombo.SelectedItem = null;
        Rows.Clear();
        foreach (var it in items.OrderBy(x => x.Name))
            Rows.Add(new Row { Item = it, IsSelected = false, Quantity = 1m });
        _view.Refresh();
    }

    private bool Filter(object obj)
    {
        if (obj is not Row r) return false;
        var q = (SearchBox.Text ?? "").Trim().ToLowerInvariant();

        if (OnlySelectedCheck.IsChecked == true && !r.IsSelected) return false;
        if (CategoryCombo.SelectedItem is LaborCategory c && r.Item.CategoryId != c.Id) return false;

        if (!string.IsNullOrEmpty(q))
        {
            var name = (r.Item.Name ?? "").ToLowerInvariant();
            var unit = (r.Item.Unit ?? "").ToLowerInvariant();
            if (!(name.Contains(q) || unit.Contains(q))) return false;
        }
        return true;
    }

    private void FilterChanged(object? s, EventArgs e) => _view.Refresh();
    private void ClearFilters_Click(object s, RoutedEventArgs e) { SearchBox.Text = ""; CategoryCombo.SelectedItem = null; OnlySelectedCheck.IsChecked = false; _view.Refresh(); }
    private void SelectAll_Click(object s, RoutedEventArgs e) { foreach (var r in Rows) r.IsSelected = true; }
    private void ClearSelection_Click(object s, RoutedEventArgs e) { foreach (var r in Rows) r.IsSelected = false; }
    private void Cancel_Click(object s, RoutedEventArgs e) { DialogResult = false; Close(); }
    private void Accept_Click(object s, RoutedEventArgs e)
    {
        Result = Rows.Where(r => r.IsSelected && r.Quantity > 0m)
                     .Select(r => new Selection { Item = r.Item, Quantity = r.Quantity })
                     .ToList();
        DialogResult = true; Close();
    }
}
