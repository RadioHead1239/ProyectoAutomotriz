using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Models
{
    public class Auditoria
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public string Accion { get; set; } = "";
        public string Entidad { get; set; } = "";
        public string? EntidadId { get; set; }
        public string? Detalle { get; set; }
    }

    public class TokenAcceso
    {
        public int Id { get; set; }
        public string Token { get; set; } = "";
        public TipoToken Tipo { get; set; }
        public int? OrdenTrabajoId { get; set; }
        public OrdenTrabajo? OrdenTrabajo { get; set; }
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public DateTime Vence { get; set; }
        public bool Usado { get; set; }
    }
}
