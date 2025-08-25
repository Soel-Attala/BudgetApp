using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presupuestos.WPF.Behaviors;

/// <summary>
/// Restringe un TextBox a números decimales (con separador '.'
/// y permite pegar/editar). No redondea ni cambia formato.
/// </summary>
public static class NumericTextBoxBehavior
{
    private static readonly Regex _rx = new(@"^[0-9]*([.][0-9]*)?$", RegexOptions.Compiled);

    public static void Attach(TextBox tb)
    {
        tb.PreviewTextInput += OnPreviewTextInput;
        DataObject.AddPastingHandler(tb, OnPaste);
    }

    public static void Detach(TextBox tb)
    {
        tb.PreviewTextInput -= OnPreviewTextInput;
        DataObject.RemovePastingHandler(tb, OnPaste);
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var tb = (TextBox)sender;
        var proposed = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength)
                             .Insert(tb.SelectionStart, e.Text);

        // Acepta cadena vacía, enteros, o decimales con '.'
        e.Handled = !_rx.IsMatch(proposed);
    }

    private static void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            var text = (string)e.DataObject.GetData(DataFormats.Text);
            if (!_rx.IsMatch(text.Replace(',', '.')))
                e.CancelCommand();
        }
        else e.CancelCommand();
    }
}
