using Microsoft.EntityFrameworkCore;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Enums;
using Presupuestos.Domain.Models;
using Presupuestos.Infrastructure.Data;

namespace Presupuestos.Infrastructure.Services;

public class BudgetService : IBudgetService
{
    private readonly AppDbContext _db;
    public BudgetService(AppDbContext db) => _db = db;

    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken ct = default)
      => await _db.Budgets
          .Include(b => b.Taxes)
          .Include(b => b.Items)
          .FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<IReadOnlyList<Budget>> GetAllAsync(CancellationToken ct = default)
      => await _db.Budgets
          .OrderByDescending(b => b.Date)
          .ThenBy(b => b.Name)
          .ToListAsync(ct);

    public async Task<IReadOnlyList<Budget>> SearchAsync(string? text, CancellationToken ct = default)
    {
        var q = _db.Budgets.AsQueryable();
        if (!string.IsNullOrWhiteSpace(text))
        {
            var t = text.Trim().ToLower();
            q = q.Where(b =>
              b.Name.ToLower().Contains(t) ||
              (b.ClientName != null && b.ClientName.ToLower().Contains(t)) ||
              (b.ClientEmail != null && b.ClientEmail.ToLower().Contains(t)));
        }
        return await q.OrderByDescending(b => b.Date).ThenBy(b => b.Name).ToListAsync(ct);
    }

    public async Task AddAsync(Budget budget, CancellationToken ct = default)
    {
        // Normalizar Items de subgrupos: garantizar header antes de hijos (opcional / recomendado)
        EnsureSubgroupOrdering(budget.Items);

        _db.Budgets.Add(budget);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Budget budget, CancellationToken ct = default)
    {
        // Traer presupuesto actual con Items
        var existing = await _db.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == budget.Id, ct);
        if (existing is null)
        {
            _db.Budgets.Add(budget);
        }
        else
        {
            // Campos simples
            existing.Name = budget.Name;
            existing.Date = budget.Date;

            existing.ClientName = budget.ClientName;
            existing.ClientEmail = budget.ClientEmail;
            existing.ClientPhone = budget.ClientPhone;
            existing.ClientAddress = budget.ClientAddress;

            existing.DollarRate = budget.DollarRate;
            existing.BenefitPercentage = budget.BenefitPercentage;
            existing.IncludeFees = budget.IncludeFees;
            existing.FeesCost = budget.FeesCost;
            existing.IncludeLabor = budget.IncludeLabor;
            existing.LaborCost = budget.LaborCost;

            // Taxes (owned)
            existing.Taxes.IncludeTaxes = budget.Taxes.IncludeTaxes;
            existing.Taxes.IVA = budget.Taxes.IVA;
            existing.Taxes.IVAT = budget.Taxes.IVAT;
            existing.Taxes.IB = budget.Taxes.IB;
            existing.Taxes.IG = budget.Taxes.IG;
            existing.Taxes.IC = budget.Taxes.IC;
            existing.Taxes.BankFees = budget.Taxes.BankFees;

            // Sync Items
            var incoming = budget.Items.ToDictionary(i => i.Id);
            foreach (var it in existing.Items.ToList())
                if (!incoming.ContainsKey(it.Id))
                    _db.BudgetItems.Remove(it);

            foreach (var it in budget.Items)
            {
                var cur = existing.Items.FirstOrDefault(x => x.Id == it.Id);
                if (cur is null)
                {
                    existing.Items.Add(it);
                }
                else
                {
                    cur.Type = it.Type;

                    cur.MaterialId = it.MaterialId;
                    cur.MaterialName = it.MaterialName;
                    cur.PriceUSD = it.PriceUSD;
                    cur.Quantity = it.Quantity;

                    cur.SubgroupId = it.SubgroupId;
                    cur.SubgroupName = it.SubgroupName;

                    cur.LaborItemId = it.LaborItemId;
                    cur.LaborItemName = it.LaborItemName;
                    cur.PriceARS = it.PriceARS;
                    cur.LaborQuantity = it.LaborQuantity;
                }
            }

            EnsureSubgroupOrdering(existing.Items);
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Budgets.FindAsync([id], ct);
        if (e is null) return;
        _db.Budgets.Remove(e);
        await _db.SaveChangesAsync(ct);
    }

    // Asegura que los SubgroupHeader permanezcan antes de sus SubgroupMaterial
    private static void EnsureSubgroupOrdering(IList<BudgetItem> items)
    {
        if (items.Count == 0) return;

        // Idea: si hay SubgroupMaterial sin header previo, opcionalmente insertar header (acá solo dejamos como están)
        // También podríamos garantizar bloques contiguos, pero lo usual es que ya vengan ordenados desde la UI.
        // Si necesitás forzar bloques, se implementa aquí.
    }
}
