using Presupuestos.Domain.Models;

namespace Presupuestos.Application.Services.Interfaces;

/// <summary>
/// Generador de reporte PDF de presupuesto.
/// Implementación en Presupuestos.Reporting (QuestPDF).
/// </summary>
public interface IBudgetReportService
{
    /// <summary>Genera un PDF del presupuesto y devuelve el contenido en memoria.</summary>
    byte[] Generate(Budget budget);
}
