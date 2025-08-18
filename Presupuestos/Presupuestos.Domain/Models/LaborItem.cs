namespace Presupuestos.Domain.Models;

public class LaborItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    /// <summary>Unidad de medida (ej: hora, m2, jornal) — opcional.</summary>
    public string? Unit { get; set; }

    /// <summary>Precio unitario en ARS (preciso, sin redondeo).</summary>
    public decimal PriceARS { get; set; }

    // Relación con categoría (opcional)
    public Guid? CategoryId { get; set; }
    public LaborCategory? Category { get; set; }
}
