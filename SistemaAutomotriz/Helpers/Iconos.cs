using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaAutomotriz.Helpers
{
    public static class Iconos
    {
        private static readonly Dictionary<string, string> Trazos = new()
        {
            ["logo"] = "<g stroke=\"#F5B544\"><circle cx=\"12\" cy=\"12\" r=\"6.2\"/><circle cx=\"12\" cy=\"12\" r=\"2.4\"/><path d=\"M12 2.6v3.2M12 18.2v3.2M2.6 12h3.2M18.2 12h3.2M5.4 5.4l2.2 2.2M16.4 16.4l2.2 2.2M5.4 18.6l2.2-2.2M16.4 7.6l2.2-2.2\"/></g>",
            ["panel"] = "<rect x=\"3\" y=\"3\" width=\"7\" height=\"9\" rx=\"1.5\"/><rect x=\"14\" y=\"3\" width=\"7\" height=\"5\" rx=\"1.5\"/><rect x=\"14\" y=\"12\" width=\"7\" height=\"9\" rx=\"1.5\"/><rect x=\"3\" y=\"16\" width=\"7\" height=\"5\" rx=\"1.5\"/>",
            ["ordenes"] = "<path d=\"M9 4h6a2 2 0 0 1 2 2v14H7V6a2 2 0 0 1 2-2z\"/><path d=\"M10 9h4M10 13h4M10 17h2\"/>",
            ["clientes"] = "<circle cx=\"9\" cy=\"8\" r=\"3.2\"/><path d=\"M3 20c0-3.5 2.7-6 6-6s6 2.5 6 6\"/><path d=\"M16 5a3 3 0 0 1 0 6M18 14c2 .6 3 2.6 3 5\"/>",
            ["stock"] = "<path d=\"M3 8l9-5 9 5v8l-9 5-9-5V8z\"/><path d=\"M3 8l9 5 9-5M12 13v8\"/>",
            ["inventario"] = "<path d=\"M3 10l2-5h14l2 5\"/><path d=\"M4 10v10h16V10\"/><path d=\"M9 20v-6h6v6\"/>",
            ["kardex"] = "<path d=\"M6 3h9l4 4v14H6V3z\"/><path d=\"M14 3v5h5M9 12h6M9 16h6\"/>",
            ["transferencias"] = "<path d=\"M4 8h14l-3-3M20 16H6l3 3\"/>",
            ["compras"] = "<path d=\"M3 4h2l2.4 11h10l2-8H6.5\"/><circle cx=\"9\" cy=\"19\" r=\"1.4\"/><circle cx=\"17\" cy=\"19\" r=\"1.4\"/>",
            ["toma"] = "<path d=\"M9 4h6a2 2 0 0 1 2 2v14H7V6a2 2 0 0 1 2-2z\"/><path d=\"M9.5 12l2 2 3.5-4\"/>",
            ["usuario"] = "<circle cx=\"12\" cy=\"8\" r=\"3.5\"/><path d=\"M5 20c0-4 3-6.5 7-6.5s7 2.5 7 6.5\"/>",
            ["auditoria"] = "<path d=\"M12 3l8 3v6c0 4.5-3.2 7.8-8 9-4.8-1.2-8-4.5-8-9V6l8-3z\"/><path d=\"M9 12l2 2 4-4\"/>",
            ["salir"] = "<path d=\"M14 4h4a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2h-4M10 16l4-4-4-4M14 12H4\"/>",
            ["chevron"] = "<path d=\"M6 9l6 6 6-6\"/>",
            ["mas"] ="<path d=\"M12 5v14M5 12h14\"/>",
            ["buscar"] = "<circle cx=\"11\" cy=\"11\" r=\"6\"/><path d=\"M20 20l-4-4\"/>",
            ["descarga"] = "<path d=\"M12 4v11M7 11l5 5 5-5M5 20h14\"/>",
            ["check"] = "<path d=\"M5 12l4 4 10-10\"/>",
            ["alerta"] = "<path d=\"M12 4l9 16H3L12 4z\"/><path d=\"M12 10v4M12 17h.01\"/>",
            ["error"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M12 8v5M12 16h.01\"/>",
            ["candado"] = "<rect x=\"5\" y=\"11\" width=\"14\" height=\"9\" rx=\"2\"/><path d=\"M8 11V8a4 4 0 0 1 8 0v3\"/>",
            ["ojo"] = "<path d=\"M2 12s3.6-7 10-7 10 7 10 7-3.6 7-10 7S2 12 2 12z\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/>",
            ["ojo-cerrado"] = "<path d=\"M3 3l18 18\"/><path d=\"M10.6 5.1A10 10 0 0 1 12 5c6.4 0 10 7 10 7a17 17 0 0 1-3.2 4.1M6.6 6.6C3.9 8.3 2 12 2 12s3.6 7 10 7a9.6 9.6 0 0 0 5.4-1.6\"/><path d=\"M9.9 9.9a3 3 0 0 0 4.2 4.2\"/>",
            ["sucursal"] = "<path d=\"M3 21h18M5 21V8l7-5 7 5v13\"/><path d=\"M9 21v-6h6v6\"/>",
            ["correo"] = "<rect x=\"3\" y=\"5\" width=\"18\" height=\"14\" rx=\"2\"/><path d=\"M3 7l9 6 9-6\"/>",
        };

        public static IHtmlContent Icono(this IHtmlHelper _, string nombre, int tamano = 20) => Svg(nombre, tamano);

        public static IHtmlContent Svg(string nombre, int tamano = 20)
        {
            var trazo = Trazos.TryGetValue(nombre, out var t) ? t : "";
            return new HtmlString(
                $"<svg width=\"{tamano}\" height=\"{tamano}\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
                $"stroke-width=\"1.8\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\">{trazo}</svg>");
        }
    }
}
