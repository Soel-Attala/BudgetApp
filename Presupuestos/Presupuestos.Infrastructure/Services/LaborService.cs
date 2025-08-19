using Microsoft.EntityFrameworkCore;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Models;
using Presupuestos.Infrastructure.Data;

namespace Presupuestos.Infrastructure.Services;

public class LaborService : ILaborService
{
    private readonly AppDbContext _db;
    public LaborService(AppDbContext db) => _db = db;

    // ===== Items =====
    public async Task<IReadOnlyList<LaborItem>> GetAllAsync(CancellationToken ct = default)
      => await _db.LaborItems.Include(x => x.Category).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<LaborItem>> SearchAsync(string? text, Guid? categoryId, CancellationToken ct = default)
    {
        var q = _db.LaborItems.Include(x => x.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(text))
        {
            var t = text.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(t) ||
                             (x.Unit != null && x.Unit.ToLower().Contains(t)));
        }

        if (categoryId is Guid cid)
            q = q.Where(x => x.CategoryId == cid);

        return await q.OrderBy(x => x.Name).ToListAsync(ct);
    }

    public async Task<LaborItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
      => await _db.LaborItems.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(LaborItem item, CancellationToken ct = default)
    {
        _db.LaborItems.Add(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(LaborItem item, CancellationToken ct = default)
    {
        _db.LaborItems.Update(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.LaborItems.FindAsync([id], ct);
        if (e is null) return;
        _db.LaborItems.Remove(e);
        await _db.SaveChangesAsync(ct);
    }

    // ===== Categorías =====
    public async Task<IReadOnlyList<LaborCategory>> GetCategoriesAsync(CancellationToken ct = default)
      => await _db.LaborCategories.OrderBy(x => x.Name).ToListAsync(ct);

    public async Task AddCategoryAsync(LaborCategory category, CancellationToken ct = default)
    {
        _db.LaborCategories.Add(category);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateCategoryAsync(LaborCategory category, CancellationToken ct = default)
    {
        _db.LaborCategories.Update(category);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.LaborCategories.FindAsync([id], ct);
        if (e is null) return;
        _db.LaborCategories.Remove(e);
        await _db.SaveChangesAsync(ct);
    }
}
