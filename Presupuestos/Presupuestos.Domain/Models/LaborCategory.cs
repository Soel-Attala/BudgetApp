namespace Presupuestos.Domain.Models;

public class LaborCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<LaborItem> Items { get; set; } = new List<LaborItem>();
}
