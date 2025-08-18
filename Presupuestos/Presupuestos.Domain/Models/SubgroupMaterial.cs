namespace Presupuestos.Domain.Models;

/// <summary>Material dentro de un Subgrupo (con cantidad por defecto y precio en USD).</summary>
public class SubgroupMaterial
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Relación al subgrupo
    public Guid SubgroupId { get; set; }

    // Material
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>Precio unitario en USD (snapshot del catálogo al momento de armar el subgrupo).</summary>
    public decimal PriceUSD { get; set; }

    /// <summary>Cantidad por defecto al importar.</summary>
    public decimal Quantity { get; set; } = 1m;
}
