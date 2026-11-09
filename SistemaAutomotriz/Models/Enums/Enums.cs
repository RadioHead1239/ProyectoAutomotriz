namespace SistemaAutomotriz.Models.Enums
{
    public enum RolUsuario { Administrador, JefeAlmacen, Recepcionista, Mecanico }

    public enum EstadoOrden { Recibida, EnDiagnostico, Cotizada, Aprobada, EnReparacion, Finalizada, Entregada }

    public enum EstadoTransferencia { Solicitada, Aprobada, Rechazada, Despachada, Recibida }

    public enum TipoMovimiento { EntradaCompra, SalidaOrden, SalidaTransferencia, EntradaTransferencia, Ajuste }

    public enum EstadoOrdenCompra { Pendiente, Recibida, Anulada }

    public enum TipoComprobante { Boleta, Factura }

    public enum EstadoTomaFisica { Borrador, Confirmada }

    public enum TipoToken { AprobarCotizacion, RecuperarClave }
}
