using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public string ClaveHash { get; set; } = "";
        public RolUsuario Rol { get; set; }
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;
        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
