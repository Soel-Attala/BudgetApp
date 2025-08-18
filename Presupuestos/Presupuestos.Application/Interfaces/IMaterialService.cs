using Presupuestos.Domain.Models;

namespace Presupuestos.Application.Services.Interfaces;

public interface IMaterialService
{
    Task<Material?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Material>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Material>> SearchAsync(string? text, CancellationToken ct = default);

    Task AddAsync(Material item, CancellationToken ct = default);
    Task UpdateAsync(Material item, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
