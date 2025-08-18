namespace Presupuestos.Domain.Enums;

public enum BudgetItemType
{
    // Material suelto del catálogo
    Material = 0,

    // Cabecera de un subgrupo (informativa, sin importes)
    SubgroupHeader = 1,

    // Material que pertenece a un subgrupo dentro del presupuesto
    SubgroupMaterial = 2,

    // Mano de obra (ítem del catálogo de MO)
    LaborItem = 3
}
