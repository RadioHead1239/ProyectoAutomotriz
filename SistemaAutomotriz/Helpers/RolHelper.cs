using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Helpers
{
    public static class RolHelper
    {
        public static readonly RolUsuario[] RolesRegistro =
            [RolUsuario.Recepcionista, RolUsuario.Mecanico, RolUsuario.JefeAlmacen];

        public static string Texto(RolUsuario rol) => rol switch
        {
            RolUsuario.Administrador => "Administrador",
            RolUsuario.JefeAlmacen => "Jefe de almacén",
            RolUsuario.Recepcionista => "Recepcionista",
            RolUsuario.Mecanico => "Mecánico",
            _ => rol.ToString()
        };

        public static string Texto(string? rol) =>
            Enum.TryParse<RolUsuario>(rol, out var r) ? Texto(r) : rol ?? "";
    }
}
