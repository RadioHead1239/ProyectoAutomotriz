using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.ViewModels
{
    public class PanelViewModel
    {
        public string Saludo { get; set; } = "";
        public string PrimerNombre { get; set; } = "";
        public string FechaTexto { get; set; } = "";
        public int SucursalId { get; set; }
        public string SucursalNombre { get; set; } = "";
        public string OtraSucursalNombre { get; set; } = "";
        public bool PuedeCambiarSucursal { get; set; }
        public IEnumerable<SelectListItem> Sucursales { get; set; } = [];

        public List<KpiItem> Kpis { get; set; } = [];

        public List<BarraItem> OrdenesPorEstado { get; set; } = [];
        public List<BarraItem> CargaMecanicos { get; set; } = [];
        public List<DiaFacturado> FacturadoSemana { get; set; } = [];
        public decimal TotalSemana { get; set; }
        public List<BarraItem> RepuestosMasUsados { get; set; } = [];
        public string MesActual { get; set; } = "";

        public List<AtencionItem> Atencion { get; set; } = [];
        public int TotalAtencion { get; set; }
        public List<AlertaStockItem> AlertasStock { get; set; } = [];
        public List<OrdenRecienteItem> OrdenesRecientes { get; set; } = [];
        public List<TransferenciaPendienteItem> Transferencias { get; set; } = [];
    }

    public record KpiItem(string Etiqueta, string Valor, string Nota, string Icono, string Tono = "");

    public record BarraItem(string Etiqueta, int Valor, string ClaseColor = "c-azul");

    public record DiaFacturado(string Dia, decimal Monto, bool EsHoy);

    public record BotonModulo(string Texto, string Controller, string Action = "Index", int? Id = null,
                              bool Primario = false, string? Icono = null, bool Pequeno = true);

    public record AtencionItem(string Titulo, string Detalle, string Tono, BotonModulo Boton);

    public record AlertaStockItem(string Repuesto, int Cantidad, int Minimo, int OtraCantidad, int OtraEnTransito);

    public record OrdenRecienteItem(int Id, string Placa, string Vehiculo, string Cliente,
                                    EstadoOrden Estado, string Mecanico, decimal Total);

    public record TransferenciaPendienteItem(string Titulo, string Detalle, BotonModulo Boton);
}
