namespace Presupuestos.Domain.Models;

/// <summary>
/// Conjunto de tasas (en %) aplicables al presupuesto. Se aplican sobre Subtotal+Beneficio.
/// </summary>
public class BudgetTaxes
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool IncludeTaxes { get; set; } = false;

    // Tasas en PORCENTAJE (no fracción)
    public decimal IVA { get; set; } = 0m;
    public decimal IVAT { get; set; } = 0m;
    public decimal IB { get; set; } = 0m;
    public decimal IG { get; set; } = 0m;
    public decimal IC { get; set; } = 0m;
    public decimal BankFees { get; set; } = 0m;

    // Fracción total (suma de tasas / 100)
    public decimal TotalRateFraction =>
      (IVA + IVAT + IB + IG + IC + BankFees) / 100m;
}
