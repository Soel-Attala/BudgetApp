namespace Presupuestos.Domain.Models;

/// <summary>
/// Subgrupo de materiales (catálogo). Al importarlo en un presupuesto,
/// se expanden sus materiales como BudgetItems (SubgroupHeader + SubgroupMaterial).
/// </summary>
public class Subgroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public List<SubgroupMaterial> Materials { get; set; } = new();
}
