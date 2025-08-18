using Presupuestos.Domain.Models;

namespace Presupuestos.Application.Services.Interfaces;

public interface IBudgetService
{
    Task<Budget?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Budget>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Budget>> SearchAsync(string? text, CancellationToken ct = default);

    Task AddAsync(Budget budget, CancellationToken ct = default);
    Task UpdateAsync(Budget budget, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
