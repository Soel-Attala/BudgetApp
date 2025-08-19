using Microsoft.EntityFrameworkCore;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Models;
using Presupuestos.Infrastructure.Data;

namespace Presupuestos.Infrastructure.Services;

public class MaterialService : IMaterialService
{
    private readonly AppDbContext _db;
    public MaterialService(AppDbContext db) => _db = db;

    public async Task<Material?> GetByIdAsync(Guid id, CancellationToken ct = default)
      => await _db.Materials.FindAsync([id], ct);

    public async Task<IReadOnlyList<Material>> GetAllAsync(CancellationToken ct = default)
      => await _db.Materials.OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<Material>> SearchAsync(string? text, CancellationToken ct = default)
    {
        var q = _db.Materials.AsQueryable();
        if (!string.IsNullOrWhiteSpace(text))
        {
            var t = text.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(t));
        }
        return await q.OrderBy(x => x.Name).ToListAsync(ct);
    }

    public async Task AddAsync(Material item, CancellationToken ct = default)
    {
        _db.Materials.Add(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Material item, CancellationToken ct = default)
    {
        _db.Materials.Update(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Materials.FindAsync([id], ct);
        if (e is null) return;
        _db.Materials.Remove(e);
        await _db.SaveChangesAsync(ct);
    }
}
