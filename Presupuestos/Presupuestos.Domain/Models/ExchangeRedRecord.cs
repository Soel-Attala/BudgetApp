namespace Presupuestos.Domain.Models;

/// <summary>
/// Registro histórico de cotización USD→ARS.
/// </summary>
public class ExchangeRateRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public string BaseCurrency { get; set; } = "USD";
    public string QuoteCurrency { get; set; } = "ARS";

    /// <summary>Tasa directa Base→Quote (precisa, sin redondeos).</summary>
    public decimal Rate { get; set; }

    public string? Provider { get; set; }
    public string? Note { get; set; }
}
