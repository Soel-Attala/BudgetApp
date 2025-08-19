using Microsoft.EntityFrameworkCore;
using Presupuestos.Application.Services.Interfaces;
using Presupuestos.Domain.Models;
using Presupuestos.Infrastructure.Data;

namespace Presupuestos.Infrastructure.Services;

public class SubgroupService : ISubgroupService
{
    private readonly AppDbContext _db;
    public SubgroupService(AppDbContext db) => _db = db;

    public async Task<Subgroup?> GetByIdAsync(Guid id, CancellationToken ct = default)
      => await _db.Subgroups.Include(s => s.Materials).FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Subgroup>> GetAllAsync(CancellationToken ct = default)
      => await _db.Subgroups.Include(s => s.Materials).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<Subgroup>> SearchAsync(string? text, CancellationToken ct = default)
    {
        var q = _db.Subgroups.Include(s => s.Materials).AsQueryable();
        if (!string.IsNullOrWhiteSpace(text))
        {
            var t = text.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(t) ||
                             x.Materials.Any(m => m.MaterialName.ToLower().Contains(t)));
        }
        return await q.OrderBy(x => x.Name).ToListAsync(ct);
    }

    public async Task AddAsync(Subgroup subgroup, CancellationToken ct = default)
    {
        _db.Subgroups.Add(subgroup);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Subgroup subgroup, CancellationToken ct = default)
    {
        // Actualizamos cabecera y materiales (simplificado)
        var existing = await _db.Subgroups.Include(s => s.Materials).FirstOrDefaultAsync(s => s.Id == subgroup.Id, ct);
        if (existing is null)
        {
            _db.Subgroups.Add(subgroup);
        }
        else
        {
            existing.Name = subgroup.Name;

            // Sincronizar materiales: eliminar faltantes, actualizar, agregar nuevos
            var incoming = subgroup.Materials.ToDictionary(m => m.Id);
            // eliminar
            foreach (var m in existing.Materials.ToList())
                if (!incoming.ContainsKey(m.Id)) _db.SubgroupMaterials.Remove(m);

            // upsert
            foreach (var m in subgroup.Materials)
            {
                var current = existing.Materials.FirstOrDefault(x => x.Id == m.Id);
                if (current is null)
                {
                    m.SubgroupId = existing.Id;
                    _db.SubgroupMaterials.Add(m);
                }
                else
                {
                    current.MaterialId = m.MaterialId;
                    current.MaterialName = m.MaterialName;
                    current.PriceUSD = m.PriceUSD;
                    current.Quantity = m.Quantity;
                }
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Subgroups.FindAsync([id], ct);
        if (e is null) return;
        _db.Subgroups.Remove(e);
        await _db.SaveChangesAsync(ct);
    }

    // ===== materiales dentro del subgrupo =====
    public async Task AddMaterialAsync(Guid subgroupId, SubgroupMaterial material, CancellationToken ct = default)
    {
        material.SubgroupId = subgroupId;
        _db.SubgroupMaterials.Add(material);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateMaterialAsync(Guid subgroupId, SubgroupMaterial material, CancellationToken ct = default)
    {
        var e = await _db.SubgroupMaterials.FirstOrDefaultAsync(x => x.Id == material.Id && x.SubgroupId == subgroupId, ct);
        if (e is null) return;
        e.MaterialId = material.MaterialId;
        e.MaterialName = material.MaterialName;
        e.PriceUSD = material.PriceUSD;
        e.Quantity = material.Quantity;
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveMaterialAsync(Guid subgroupId, Guid subgroupMaterialId, CancellationToken ct = default)
    {
        var e = await _db.SubgroupMaterials.FirstOrDefaultAsync(x => x.Id == subgroupMaterialId && x.SubgroupId == subgroupId, ct);
        if (e is null) return;
        _db.SubgroupMaterials.Remove(e);
        await _db.SaveChangesAsync(ct);
    }
}
