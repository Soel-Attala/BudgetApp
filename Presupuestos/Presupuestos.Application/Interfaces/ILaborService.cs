using Presupuestos.Domain.Models;

namespace Presupuestos.Application.Services.Interfaces;

public interface ILaborService
{
    // Items
    Task<IReadOnlyList<LaborItem>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LaborItem>> SearchAsync(string? text, Guid? categoryId, CancellationToken ct = default);
    Task<LaborItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(LaborItem item, CancellationToken ct = default);
    Task UpdateAsync(LaborItem item, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    // Categorías
    Task<IReadOnlyList<LaborCategory>> GetCategoriesAsync(CancellationToken ct = default);
    Task AddCategoryAsync(LaborCategory category, CancellationToken ct = default);
    Task UpdateCategoryAsync(LaborCategory category, CancellationToken ct = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken ct = default);
}
