using System.Globalization;
using System.Windows.Controls;

namespace Presupuestos.WPF.Validation;

/// <summary>
/// Valida que el valor sea decimal >= 0 (sin redondear).
/// </summary>
public class DecimalNonNegativeRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var s = (value as string)?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(s)) return ValidationResult.ValidResult;

        s = s.Replace(',', '.');
        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var d) && d >= 0m)
            return ValidationResult.ValidResult;

        return new ValidationResult(false, "Ingrese un número válido (≥ 0)");
    }
}
