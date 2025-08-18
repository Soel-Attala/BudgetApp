namespace Presupuestos.Domain.Models;

public class Material
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    /// <summary>Precio unitario en USD (preciso, sin redondeo).</summary>
    public decimal PriceUSD { get; set; } = 0m;

    // Campos opcionales futuros: SKU, Marca, Unidad, etc.
}
