using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Models;
using Presupuestos.Infrastructure.Data;

namespace Presupuestos.Infrastructure.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _http;

    public ExchangeRateService(AppDbContext db, HttpClient http)
    {
        _db = db;
        _http = http;
        _http.Timeout = TimeSpan.FromSeconds(12);
    }

    public async Task<ExchangeRateRecord?> GetLastAsync(CancellationToken ct = default)
      => await _db.ExchangeRates.OrderByDescending(x => x.TimestampUtc).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ExchangeRateRecord>> GetRecentAsync(int take = 10, CancellationToken ct = default)
      => await _db.ExchangeRates.OrderByDescending(x => x.TimestampUtc).Take(take).ToListAsync(ct);

    public async Task<ExchangeRateRecord?> RefreshUsdArsAsync(string? provider = null, CancellationToken ct = default)
    {
        ExchangeRateRecord? rec = null;

        switch ((provider ?? "").Trim().ToLowerInvariant())
        {
            case "exhost":
            case "exchangerate.host":
                rec = await TryExchangerateHost(ct);
                break;

            case "oficial":
            case "dolarsi:oficial":
                rec = await TryDolarsi(ct, preferBlue: false);
                break;

            case "blue":
            case "dolarsi:blue":
                rec = await TryDolarsi(ct, preferBlue: true);
                break;

            case "":
                rec = await TryExchangerateHost(ct)
                   ?? await TryDolarsi(ct, preferBlue: false)
                   ?? await TryDolarsi(ct, preferBlue: true);
                break;

            default:
                if (provider.StartsWith("dolarsi:", StringComparison.OrdinalIgnoreCase))
                    rec = await TryDolarsi(ct, preferBlue: provider.ToLowerInvariant().Contains("blue"));
                break;
        }

        if (rec is null) return null;

        _db.ExchangeRates.Add(rec);
        await _db.SaveChangesAsync(ct);
        return rec;
    }

    // ====== Providers ======

    private async Task<ExchangeRateRecord?> TryExchangerateHost(CancellationToken ct)
    {
        try
        {
            var url = "https://api.exchangerate.host/latest?base=USD&symbols=ARS";
            var json = await _http.GetFromJsonAsync<ExHostResponse>(url, ct);
            if (json?.rates is null) return null;
            if (!json.rates.TryGetValue("ARS", out var rate)) return null;

            return new ExchangeRateRecord
            {
                BaseCurrency = "USD",
                QuoteCurrency = "ARS",
                Rate = (decimal)rate,
                Provider = "exchangerate.host",
                TimestampUtc = DateTime.UtcNow
            };
        }
        catch { return null; }
    }

    private async Task<ExchangeRateRecord?> TryDolarsi(CancellationToken ct, bool preferBlue)
    {
        try
        {
            var url = "https://www.dolarsi.com/api/api.php?type=valoresprincipales";
            var list = await _http.GetFromJsonAsync<List<DolarsiItem>>(url, ct);
            if (list is null || list.Count == 0) return null;

            Func<string?, bool> isOficial = n => (n ?? "").Contains("Oficial", StringComparison.OrdinalIgnoreCase);
            Func<string?, bool> isBlue = n => (n ?? "").Contains("Blue", StringComparison.OrdinalIgnoreCase);

            var oficialCasa = list.FirstOrDefault(x => isOficial(x.casa?.nombre))?.casa;
            var blueCasa = list.FirstOrDefault(x => isBlue(x.casa?.nombre))?.casa;

            var casa = preferBlue ? blueCasa ?? oficialCasa : oficialCasa ?? blueCasa;
            if (casa is null) return null;

            decimal Parse(string s)
            {
                s = s.Replace(".", "").Replace(",", ".");
                return decimal.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : 0m;
            }

            var venta = Parse(casa.venta ?? "0");
            var compra = Parse(casa.compra ?? "0");
            var rate = venta > 0 ? venta : compra;

            if (rate <= 0) return null;

            return new ExchangeRateRecord
            {
                BaseCurrency = "USD",
                QuoteCurrency = "ARS",
                Rate = rate,
                Provider = $"dolarsi:{casa.nombre}",
                TimestampUtc = DateTime.UtcNow
            };
        }
        catch { return null; }
    }

    // ====== DTOs ======
    private class ExHostResponse
    {
        public string? @base { get; set; }
        public Dictionary<string, double>? rates { get; set; }
        public string? date { get; set; }
    }

    private class DolarsiItem
    {
        public Casa? casa { get; set; }
        public class Casa
        {
            public string? nombre { get; set; }
            public string? compra { get; set; }
            public string? venta { get; set; }
        }
    }
}
