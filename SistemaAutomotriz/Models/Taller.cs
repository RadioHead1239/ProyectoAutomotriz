using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Documento { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }

    public class Vehiculo
    {
        public int Id { get; set; }
        public string Placa { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int Anio { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public ICollection<OrdenTrabajo> Ordenes { get; set; } = new List<OrdenTrabajo>();
    }

    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public decimal PrecioBase { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class OrdenTrabajo
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public EstadoOrden Estado { get; set; } = EstadoOrden.Recibida;
        public string? Diagnostico { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo Vehiculo { get; set; } = null!;
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;
        public int? MecanicoId { get; set; }
        public Usuario? Mecanico { get; set; }

        public ICollection<OrdenServicio> Servicios { get; set; } = new List<OrdenServicio>();
        public ICollection<OrdenRepuesto> Repuestos { get; set; } = new List<OrdenRepuesto>();
    }

    public class OrdenServicio
    {
        public int Id { get; set; }
        public int OrdenTrabajoId { get; set; }
        public OrdenTrabajo OrdenTrabajo { get; set; } = null!;
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; } = null!;
        public decimal Precio { get; set; }
    }

    public class OrdenRepuesto
    {
        public int Id { get; set; }
        public int OrdenTrabajoId { get; set; }
        public OrdenTrabajo OrdenTrabajo { get; set; } = null!;
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class Comprobante
    {
        public int Id { get; set; }
        public TipoComprobante Tipo { get; set; }
        public string Serie { get; set; } = "";
        public int Correlativo { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public int OrdenTrabajoId { get; set; }
        public OrdenTrabajo OrdenTrabajo { get; set; } = null!;
    }
}
