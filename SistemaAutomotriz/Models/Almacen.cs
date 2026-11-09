using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Ruc { get; set; } = "";
        public string RazonSocial { get; set; } = "";
        public string? Contacto { get; set; }
    }

    public class Repuesto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Categoria { get; set; } = "";
        public decimal PrecioVenta { get; set; }
        public int StockMinimo { get; set; }
        public int? ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

        public ICollection<StockSucursal> Stocks { get; set; } = new List<StockSucursal>();
    }

    public class StockSucursal
    {
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; } = null!;
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;
        public int Cantidad { get; set; }
        public int EnTransito { get; set; }
    }

    public class MovimientoKardex
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public TipoMovimiento Tipo { get; set; }
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; } = null!;
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;
        public int Cantidad { get; set; }
        public int SaldoFinal { get; set; }
        public string? Referencia { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }

    public class Transferencia
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public EstadoTransferencia Estado { get; set; } = EstadoTransferencia.Solicitada;
        public int SucursalOrigenId { get; set; }
        public Sucursal SucursalOrigen { get; set; } = null!;
        public int SucursalDestinoId { get; set; }
        public Sucursal SucursalDestino { get; set; } = null!;
        public int SolicitanteId { get; set; }
        public Usuario Solicitante { get; set; } = null!;

        public ICollection<TransferenciaDetalle> Detalles { get; set; } = new List<TransferenciaDetalle>();
    }

    public class TransferenciaDetalle
    {
        public int Id { get; set; }
        public int TransferenciaId { get; set; }
        public Transferencia Transferencia { get; set; } = null!;
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; } = null!;
        public int Cantidad { get; set; }
    }

    public class OrdenCompra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public EstadoOrdenCompra Estado { get; set; } = EstadoOrdenCompra.Pendiente;
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; } = null!;
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;

        public ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();
    }

    public class OrdenCompraDetalle
    {
        public int Id { get; set; }
        public int OrdenCompraId { get; set; }
        public OrdenCompra OrdenCompra { get; set; } = null!;
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
    }

    public class TomaFisica
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public EstadoTomaFisica Estado { get; set; } = EstadoTomaFisica.Borrador;

        public ICollection<TomaFisicaDetalle> Detalles { get; set; } = new List<TomaFisicaDetalle>();
    }

    public class TomaFisicaDetalle
    {
        public int Id { get; set; }
        public int TomaFisicaId { get; set; }
        public TomaFisica TomaFisica { get; set; } = null!;
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; } = null!;
        public int CantidadSistema { get; set; }
        public int CantidadContada { get; set; }
        public int Diferencia { get; set; }
    }
}
