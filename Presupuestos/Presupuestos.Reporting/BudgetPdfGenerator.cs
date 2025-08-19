using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Presupuestos.Domain.Enums;
using Presupuestos.Domain.Models;
using Presupuestos.Application.Services.Interfaces;

namespace Presupuestos.Reporting;

/// <summary>
/// Genera un PDF de 2+ páginas:
/// - Página 1: Portada (logo, empresa, presupuesto, cliente).
/// - Página 2+: Detalle (Materiales, Mano de Obra, Subtotales, Total, Notas).
/// </summary>
public class BudgetPdfGenerator : IBudgetReportService
{
    // ===== Branding (placeholders: cambiá cuando tengas los datos reales) =====
    private const string Company_Name = "Mi Empresa S.R.L.";
    private const string Company_Tagline = "Soluciones a medida";
    private const string Company_Address = "Av. Siempre Viva 742, Mendoza, AR";
    private const string Company_Phone = "+54 261 555-1234";
    private const string Company_Email = "contacto@miempresa.com";

    // El WPF copia /Assets al lado del .exe; acá buscamos ese logo.
    private static string LogoPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "logo.png");

    public byte[] Generate(Budget budget)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var doc = Document.Create(container =>
        {
            // ========= PÁGINA 1: PORTADA =========
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.Background(Colors.White);

                page.Header().Element(c => HeaderBar(c, isCover: true));
                page.Content().Element(c => CoverContent(c, budget));
                page.Footer().Element(FooterBar);
            });

            // ========= PÁGINAS 2+: DETALLE =========
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Header().Element(c => HeaderBar(c, isCover: false));

                page.Content().Column(col =>
                {
                    col.Spacing(12);

                    // Info superior del presupuesto
                    col.Item().Element(c => BudgetInfo(c, budget));

                    // Materiales
                    var hasMaterials = budget.Items.Any(i => i.Type is BudgetItemType.Material or BudgetItemType.SubgroupMaterial);
                    if (hasMaterials)
                    {
                        col.Item().Text("Materiales").Bold().FontSize(14).FontColor(Colors.Grey.Darken3);
                        col.Item().Element(c => MaterialsTable(c, budget));
                    }

                    // Mano de Obra
                    var hasLabor = budget.Items.Any(i => i.Type == BudgetItemType.LaborItem);
                    if (hasLabor)
                    {
                        col.Item().PaddingTop(6).Text("Mano de Obra").Bold().FontSize(14).FontColor(Colors.Grey.Darken3);
                        col.Item().Element(c => LaborTable(c, budget));
                    }

                    // Resumen y total
                    col.Item().PaddingTop(6).Element(c => TotalsSummary(c, budget));

                    // Notas / Condiciones
                    col.Item().PaddingTop(8).Element(NotesBlock);
                });

                page.Footer().Element(FooterBar);
            });
        });

        return doc.GeneratePdf();
    }

    // ========================= Secciones =========================

    private void HeaderBar(IContainer container, bool isCover)
    {
        container.Row(r =>
        {
            r.ConstantItem(80).Height(40).Element(LogoBox);
            r.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text(Company_Name).SemiBold().FontSize(isCover ? 14 : 12);
                if (!string.IsNullOrWhiteSpace(Company_Tagline))
                    col.Item().Text(Company_Tagline).FontSize(9).FontColor(Colors.Grey.Darken2);
            });
        });
    }

    private void FooterBar(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text($"{Company_Address} | {Company_Phone} | {Company_Email}")
              .FontSize(9).FontColor(Colors.Grey.Darken2);

            row.ConstantItem(120).AlignRight().Text(txt =>
            {
                txt.Span("Página ");
                txt.CurrentPageNumber();
                txt.Span(" de ");
                txt.TotalPages();
            }).FontSize(9).FontColor(Colors.Grey.Darken2);
        });
    }

    private void CoverContent(IContainer container, Budget budget)
    {
        container.PaddingTop(20).Column(col =>
        {
            col.Spacing(12);

            // Título
            col.Item().AlignCenter().Text("Presupuesto")
              .FontSize(30).SemiBold().FontColor(Colors.Grey.Darken3);

            // Nombre del presupuesto
            col.Item().AlignCenter().Text(budget.Name ?? "Sin título")
              .FontSize(18).FontColor(Colors.Grey.Darken1);

            // Separador
            col.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

            // Tarjetas: Empresa / Presupuesto / Cliente
            col.Item().PaddingTop(16).Grid(g =>
            {
                g.Columns(3);

                // Empresa
                g.Item().Column(c =>
                {
                    c.Item().Text("Datos de la empresa").Bold().FontSize(12);
                    c.Item().Text(Company_Name);
                    if (!string.IsNullOrWhiteSpace(Company_Address)) c.Item().Text(Company_Address);
                    if (!string.IsNullOrWhiteSpace(Company_Phone)) c.Item().Text($"Tel: {Company_Phone}");
                    if (!string.IsNullOrWhiteSpace(Company_Email)) c.Item().Text($"Email: {Company_Email}");
                });

                // Presupuesto
                g.Item().Column(c =>
                {
                    c.Item().Text("Datos del presupuesto").Bold().FontSize(12);
                    c.Item().Text($"Fecha: {budget.Date:dd/MM/yyyy}");
                    c.Item().Text($"Cotización USD→ARS: {budget.DollarRate:N6}");
                    c.Item().Text($"Beneficio: {budget.BenefitPercentage:N2}%");
                    c.Item().Text($"Honorarios incluidos: {(budget.IncludeFees ? "Sí" : "No")}");
                    c.Item().Text($"MO manual incluida: {(budget.IncludeLabor ? "Sí" : "No")}");
                });

                // Cliente
                g.Item().Column(c =>
                {
                    c.Item().Text("Cliente").Bold().FontSize(12);
                    c.Item().Text(string.IsNullOrWhiteSpace(budget.ClientName) ? "-" : budget.ClientName);
                    if (!string.IsNullOrWhiteSpace(budget.ClientAddress)) c.Item().Text(budget.ClientAddress);
                    if (!string.IsNullOrWhiteSpace(budget.ClientPhone)) c.Item().Text($"Tel: {budget.ClientPhone}");
                    if (!string.IsNullOrWhiteSpace(budget.ClientEmail)) c.Item().Text($"Email: {budget.ClientEmail}");
                });
            });

            // Resumen rápido
            col.Item().PaddingTop(24).Box().Background(Colors.Grey.Lighten4).Padding(12).Column(c =>
            {
                c.Spacing(4);
                c.Item().Text("Resumen rápido").Bold().FontSize(12);

                var (materials, laborCatalog, laborManual, fees, baseSubtotal, benefit, subtotalPlusBenefit, taxSum, total)
                  = ComputeTotals(budget);

                c.Item().Row(r => { r.RelativeItem().Text("Materiales (ARS):"); r.ConstantItem(140).AlignRight().Text(materials.ToString("N2")); });
                if (laborCatalog > 0) c.Item().Row(r => { r.RelativeItem().Text("Mano de Obra (detalle) ARS:"); r.ConstantItem(140).AlignRight().Text(laborCatalog.ToString("N2")); });
                if (laborManual > 0) c.Item().Row(r => { r.RelativeItem().Text("Mano de Obra (manual) ARS:"); r.ConstantItem(140).AlignRight().Text(laborManual.ToString("N2")); });
                if (fees > 0) c.Item().Row(r => { r.RelativeItem().Text("Honorarios (ARS):"); r.ConstantItem(140).AlignRight().Text(fees.ToString("N2")); });

                c.Item().Row(r => { r.RelativeItem().Text("Subtotal:"); r.ConstantItem(140).AlignRight().Text(baseSubtotal.ToString("N2")); });
                c.Item().Row(r => { r.RelativeItem().Text($"Beneficio ({budget.BenefitPercentage:N2}%):"); r.ConstantItem(140).AlignRight().Text(benefit.ToString("N2")); });
                c.Item().Row(r => { r.RelativeItem().Text("Subtotal + Beneficio:"); r.ConstantItem(140).AlignRight().Text(subtotalPlusBenefit.ToString("N2")); });

                if (budget.Taxes?.IncludeTaxes == true && taxSum > 0)
                    c.Item().Row(r => { r.RelativeItem().Text("Impuestos:"); r.ConstantItem(140).AlignRight().Text(taxSum.ToString("N2")); });

                c.Item().PaddingTop(4).BorderTop(1).BorderColor(Colors.Grey.Lighten1);
                c.Item().Row(r =>
                {
                    r.RelativeItem().Text("TOTAL (ARS)").Bold();
                    r.ConstantItem(140).AlignRight().Text(total.ToString("N2")).Bold();
                });
            });

            // Observaciones
            col.Item().PaddingTop(16).Text("Observaciones:").Italic().FontColor(Colors.Grey.Darken2);
            col.Item().Text("— Presupuesto válido por 15 días si no se indica lo contrario.")
                       .FontSize(10).FontColor(Colors.Grey.Darken2);
            col.Item().Text("— Los precios en USD se convierten a ARS con la cotización indicada.")
                       .FontSize(10).FontColor(Colors.Grey.Darken2);
        });
    }

    private void BudgetInfo(IContainer container, Budget budget)
    {
        container.Box().Background(Colors.Grey.Lighten4).Padding(8).Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(budget.Name ?? "Sin título").SemiBold().FontSize(12);
                    c.Item().Text($"Fecha: {budget.Date:dd/MM/yyyy}");
                    if (!string.IsNullOrWhiteSpace(budget.ClientName))
                        c.Item().Text($"Cliente: {budget.ClientName}").FontSize(10);
                });
                row.ConstantItem(220).Column(c =>
                {
                    c.Item().AlignRight().Text($"USD→ARS: {budget.DollarRate:N6}").FontSize(10);
                    c.Item().AlignRight().Text($"Beneficio: {budget.BenefitPercentage:N2}%").FontSize(10);
                });
            });

            if (!string.IsNullOrWhiteSpace(budget.ClientEmail) || !string.IsNullOrWhiteSpace(budget.ClientPhone) || !string.IsNullOrWhiteSpace(budget.ClientAddress))
            {
                col.Item().PaddingTop(4).Row(r =>
                {
                    r.RelativeItem().Text($"{budget.ClientAddress ?? ""}".Trim());
                    r.ConstantItem(220).AlignRight().Text($"{budget.ClientPhone ?? ""}  {budget.ClientEmail ?? ""}".Trim());
                }).DefaultTextStyle(s => s.FontSize(9).FontColor(Colors.Grey.Darken2));
            }
        });
    }

    private void MaterialsTable(IContainer container, Budget budget)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(28);   // #
                c.RelativeColumn(5);    // Descripción
                c.ConstantColumn(70);   // Cant.
                c.ConstantColumn(65);   // USD
                c.ConstantColumn(80);   // Unit ARS
                c.ConstantColumn(90);   // Subtotal ARS
            });

            table.Header(h =>
            {
                CellHeader(h.Cell(), "#");
                CellHeader(h.Cell(), "Descripción");
                CellHeader(h.Cell(), "Cant.");
                CellHeader(h.Cell(), "USD");
                CellHeader(h.Cell(), "Unit ARS");
                CellHeader(h.Cell(), "Subtotal ARS");
            });

            int idx = 1;
            foreach (var it in budget.Items)
            {
                if (it.Type == BudgetItemType.SubgroupHeader)
                {
                    table.Cell().ColumnSpan(6)
                         .Element(SubgroupHeaderStyle)
                         .Text($"Subgrupo: {it.SubgroupName}")
                         .SemiBold();
                    continue;
                }

                if (it.Type is BudgetItemType.Material or BudgetItemType.SubgroupMaterial)
                {
                    var qty = it.Quantity ?? 0m;
                    var usd = it.PriceUSD ?? 0m;
                    var unitArs = usd * budget.DollarRate;    // preciso (sin redondeos internos)
                    var subtotal = unitArs * qty;

                    Cell(table.Cell(), idx++.ToString());
                    Cell(table.Cell(), it.MaterialName ?? "-");
                    Cell(table.Cell(), qty.ToString("N2"));
                    Cell(table.Cell(), usd.ToString("N2"));
                    Cell(table.Cell(), unitArs.ToString("N6"));
                    Cell(table.Cell(), subtotal.ToString("N2"));
                }
            }
        });
    }

    private void LaborTable(IContainer container, Budget budget)
    {
        var labor = budget.Items.Where(i => i.Type == BudgetItemType.LaborItem).ToList();

        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(28);   // #
                c.RelativeColumn(5);    // Ítem
                c.ConstantColumn(80);   // Unit ARS
                c.ConstantColumn(70);   // Cant.
                c.ConstantColumn(95);   // Subtotal
            });

            table.Header(h =>
            {
                CellHeader(h.Cell(), "#");
                CellHeader(h.Cell(), "Ítem");
                CellHeader(h.Cell(), "Unit ARS");
                CellHeader(h.Cell(), "Cant.");
                CellHeader(h.Cell(), "Subtotal ARS");
            });

            int idx = 1;
            foreach (var it in labor)
            {
                var unit = it.PriceARS ?? 0m;              // preciso
                var qty = it.LaborQuantity ?? 0m;
                var sub = unit * qty;

                Cell(table.Cell(), idx++.ToString());
                Cell(table.Cell(), it.LaborItemName ?? "-");
                Cell(table.Cell(), unit.ToString("N6"));
                Cell(table.Cell(), qty.ToString("N2"));
                Cell(table.Cell(), sub.ToString("N2"));
            }
        });
    }

    private void TotalsSummary(IContainer container, Budget budget)
    {
        var (materials, laborCatalog, laborManual, fees, baseSubtotal, benefit, subtotalPlusBenefit, taxSum, total)
          = ComputeTotals(budget);

        container.Row(row =>
        {
            row.RelativeItem();

            row.ConstantItem(280).Column(col =>
            {
                col.Spacing(3);

                col.Item().Row(r => { r.RelativeItem().Text("Total Materiales (ARS):"); r.ConstantItem(140).AlignRight().Text(materials.ToString("N2")); });
                if (laborCatalog > 0) col.Item().Row(r => { r.RelativeItem().Text("Mano de Obra (detalle) ARS:"); r.ConstantItem(140).AlignRight().Text(laborCatalog.ToString("N2")); });
                if (laborManual > 0) col.Item().Row(r => { r.RelativeItem().Text("Mano de Obra (manual) ARS:"); r.ConstantItem(140).AlignRight().Text(laborManual.ToString("N2")); });
                if (fees > 0) col.Item().Row(r => { r.RelativeItem().Text("Honorarios (ARS):"); r.ConstantItem(140).AlignRight().Text(fees.ToString("N2")); });

                col.Item().PaddingTop(4).BorderTop(1).BorderColor(Colors.Grey.Lighten2);
                col.Item().Row(r => { r.RelativeItem().Text("Subtotal:"); r.ConstantItem(140).AlignRight().Text(baseSubtotal.ToString("N2")); });
                col.Item().Row(r => { r.RelativeItem().Text($"Beneficio ({budget.BenefitPercentage:N2}%):"); r.ConstantItem(140).AlignRight().Text(benefit.ToString("N2")); });
                col.Item().Row(r => { r.RelativeItem().Text("Subtotal + Beneficio:"); r.ConstantItem(140).AlignRight().Text(subtotalPlusBenefit.ToString("N2")); });

                if (budget.Taxes?.IncludeTaxes == true && taxSum > 0)
                    col.Item().Row(r => { r.RelativeItem().Text("Impuestos:"); r.ConstantItem(140).AlignRight().Text(taxSum.ToString("N2")); });

                col.Item().PaddingTop(5).Box().Background(Colors.Blue.Medium).Padding(8).Row(r =>
                {
                    r.RelativeItem().Text("TOTAL (ARS)").Bold().FontColor(Colors.White);
                    r.ConstantItem(140).AlignRight().Text(total.ToString("N2")).Bold().FontColor(Colors.White);
                });
            });
        });
    }

    private void NotesBlock(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(2);
            col.Item().Text("Condiciones / Notas").SemiBold().FontSize(11);
            col.Item().Text("• Presupuesto válido por 15 días, excepto acuerdo en contrario.").FontSize(10);
            col.Item().Text("• Los precios en USD se convierten a ARS con la cotización indicada.").FontSize(10);
            col.Item().Text("• Impuestos aplicables según corresponda.").FontSize(10);
        });
    }

    // ========================= Helpers de estilo =========================

    // Encabezado de celda (fix: sin DefaultTextStyle)
    private void CellHeader(IContainer c, string text)
    {
        c.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingVertical(4)
         .Text(text).SemiBold();
    }

    // Celda normal
    private void Cell(IContainer c, string text)
      => c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2).Text(text);

    // Estilo para cabecera de subgrupo (fix: sin DefaultTextStyle)
    private static IContainer SubgroupHeaderStyle(IContainer c)
      => c.Background(Colors.Grey.Lighten3).Padding(4);

    private void LogoBox(IContainer c)
    {
        if (File.Exists(LogoPath))
        {
            var bytes = File.ReadAllBytes(LogoPath);
            c.AlignLeft().AlignMiddle().Width(80).Height(40).Image(bytes, ImageScaling.FitArea);
        }
        else
        {
            c.AlignLeft().AlignMiddle().Width(80).Height(40).Placeholder();
        }
    }

    // ========================= Cálculos =========================

    /// Devuelve tupla con:
    /// materials, laborCatalog, laborManual, fees, baseSubtotal, benefit, subtotalPlusBenefit, taxSum, total
    private (decimal materials, decimal laborCatalog, decimal laborManual, decimal fees,
             decimal baseSubtotal, decimal benefit, decimal subtotalPlusBenefit, decimal taxSum, decimal total)
      ComputeTotals(Budget budget)
    {
        decimal materials = budget.Items
          .Where(i => i.Type is BudgetItemType.Material or BudgetItemType.SubgroupMaterial)
          .Sum(i => (i.PriceUSD ?? 0m) * (i.Quantity ?? 0m) * budget.DollarRate);

        decimal laborCatalog = budget.Items
          .Where(i => i.Type == BudgetItemType.LaborItem)
          .Sum(i => (i.PriceARS ?? 0m) * (i.LaborQuantity ?? 0m)); // preciso

        decimal laborManual = budget.IncludeLabor ? budget.LaborCost : 0m;
        decimal fees = budget.IncludeFees ? budget.FeesCost : 0m;

        decimal baseSubtotal = materials + laborCatalog + laborManual + fees;
        decimal benefit = baseSubtotal * (budget.BenefitPercentage / 100m);
        decimal subtotalPlusBenefit = baseSubtotal + benefit;

        decimal taxSum = 0m;
        if (budget.Taxes?.IncludeTaxes == true)
        {
            taxSum += subtotalPlusBenefit * (budget.Taxes.IVA / 100m);
            taxSum += subtotalPlusBenefit * (budget.Taxes.IVAT / 100m);
            taxSum += subtotalPlusBenefit * (budget.Taxes.IB / 100m);
            taxSum += subtotalPlusBenefit * (budget.Taxes.IG / 100m);
            taxSum += subtotalPlusBenefit * (budget.Taxes.IC / 100m);
            taxSum += subtotalPlusBenefit * (budget.Taxes.BankFees / 100m);
        }

        decimal total = subtotalPlusBenefit + taxSum;
        return (materials, laborCatalog, laborManual, fees, baseSubtotal, benefit, subtotalPlusBenefit, taxSum, total);
    }
}
