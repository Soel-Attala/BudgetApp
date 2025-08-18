using Presupuestos.Domain.Models;

namespace Presupuestos.Application.Services.Interfaces;

/// <summary>
/// Servicio para obtener cotización USD→ARS y mantener historial.
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Trae USD→ARS según provider y guarda en historial.
    /// provider: "exhost" | "dolarsi:oficial" | "dolarsi:blue" | null/"" (fallback).
    /// </summary>
    Task<ExchangeRateRecord?> RefreshUsdArsAsync(string? provider = null, CancellationToken ct = default);

    /// <summary>Últimos N registros (orden desc).</summary>
    Task<IReadOnlyList<ExchangeRateRecord>> GetRecentAsync(int take = 10, CancellationToken ct = default);

    /// <summary>Último registro local, sin consultar online.</summary>
    Task<ExchangeRateRecord?> GetLastAsync(CancellationToken ct = default);
}
