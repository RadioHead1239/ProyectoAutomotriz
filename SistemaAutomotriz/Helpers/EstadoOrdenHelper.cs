using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Helpers
{
    public static class EstadoOrdenHelper
    {
        public static readonly EstadoOrden[] EnCurso =
            [EstadoOrden.Recibida, EstadoOrden.EnDiagnostico, EstadoOrden.Cotizada, EstadoOrden.Aprobada, EstadoOrden.EnReparacion];

        public static string Texto(EstadoOrden e) => e switch
        {
            EstadoOrden.Recibida => "Recibida",
            EstadoOrden.EnDiagnostico => "En diagnóstico",
            EstadoOrden.Cotizada => "Cotizada",
            EstadoOrden.Aprobada => "Aprobada",
            EstadoOrden.EnReparacion => "En reparación",
            EstadoOrden.Finalizada => "Finalizada",
            EstadoOrden.Entregada => "Entregada",
            _ => e.ToString()
        };

        public static string ClaseBadge(EstadoOrden e) => "estado-" + e.ToString().ToLowerInvariant();

        public static string ClaseBarra(EstadoOrden e) => e switch
        {
            EstadoOrden.Recibida => "c-gris",
            EstadoOrden.EnDiagnostico => "c-azul",
            EstadoOrden.Cotizada => "c-ambar",
            EstadoOrden.Aprobada => "c-morado",
            EstadoOrden.EnReparacion => "c-naranja",
            EstadoOrden.Finalizada => "c-verde",
            _ => "c-gris"
        };
    }
}
