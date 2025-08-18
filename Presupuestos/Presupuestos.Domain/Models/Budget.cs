using Presupuestos.Domain.Enums;

namespace Presupuestos.Domain.Models;

public class Budget
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Identificación
    public string Name { get; set; } = "Nuevo Presupuesto";
    public DateTime Date { get; set; } = DateTime.Now;

    // Cliente (opcionales)
    public string? ClientName { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientPhone { get; set; }
    public string? ClientAddress { get; set; }

    // Parámetros económicos
    public decimal DollarRate { get; set; } = 1m;         // USD→ARS (preciso, sin redondeo)
    public decimal BenefitPercentage { get; set; } = 0m;  // %
    public bool IncludeFees { get; set; } = false;
    public decimal FeesCost { get; set; } = 0m;           // ARS
    public bool IncludeLabor { get; set; } = false;
    public decimal LaborCost { get; set; } = 0m;          // ARS

    // Ítems del presupuesto (materiales, subgrupos, mano de obra)
    public List<BudgetItem> Items { get; set; } = new();

    // Impuestos
    public BudgetTaxes Taxes { get; set; } = new();

    // Helpers de cálculo (no mapeados si usás EF, podés marcarlos como [NotMapped])
    public decimal TotalMaterialsARS =>
      Items.Where(i => i.Type is BudgetItemType.Material or BudgetItemType.SubgroupMaterial)
           .Sum(i => (i.PriceUSD ?? 0m) * (i.Quantity ?? 0m) * DollarRate);

    public decimal TotalLaborCatalogARS =>
      Items.Where(i => i.Type == BudgetItemType.LaborItem)
           .Sum(i => (i.PriceARS ?? 0m) * (i.LaborQuantity ?? 0m));

    public decimal BaseSubtotal =>
      TotalMaterialsARS + TotalLaborCatalogARS +
      (IncludeLabor ? LaborCost : 0m) +
      (IncludeFees ? FeesCost : 0m);

    public decimal BenefitAmount => BaseSubtotal * (BenefitPercentage / 100m);
    public decimal SubtotalPlusBenefit => BaseSubtotal + BenefitAmount;

    public decimal TaxesAmount => Taxes.IncludeTaxes
      ? Taxes.TotalRateFraction * SubtotalPlusBenefit
      : 0m;

    public decimal Total => SubtotalPlusBenefit + TaxesAmount;
}
