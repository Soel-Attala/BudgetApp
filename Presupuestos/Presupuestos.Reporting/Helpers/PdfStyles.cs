using QuestPDF.Helpers;

namespace Presupuestos.Reporting.Helpers;

public static class PdfStyles
{
    // Colores corporativos (ajustá cuando definas branding)
    public static class Palette
    {
        public static string Primary => Colors.Blue.Medium;
        public static string Accent => Colors.Grey.Lighten4;
        public static string TextDark => Colors.Grey.Darken3;
        public static string TextSoft => Colors.Grey.Darken2;
        public static string BorderSoft => Colors.Grey.Lighten2;
    }
}
