using Presupuestos.Domain.Models;

namespace Presupuestos.Application.Services.Interfaces;

public interface ISubgroupService
{
    Task<Subgroup?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Subgroup>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Subgroup>> SearchAsync(string? text, CancellationToken ct = default);

    Task AddAsync(Subgroup subgroup, CancellationToken ct = default);
    Task UpdateAsync(Subgroup subgroup, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    // Materiales dentro del subgrupo
    Task AddMaterialAsync(Guid subgroupId, SubgroupMaterial material, CancellationToken ct = default);
    Task UpdateMaterialAsync(Guid subgroupId, SubgroupMaterial material, CancellationToken ct = default);
    Task RemoveMaterialAsync(Guid subgroupId, Guid subgroupMaterialId, CancellationToken ct = default);
}
