using System.Security.Claims;
using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Helpers
{
    public record MenuItem(string Texto, string Controller, string Icono, bool Construido,
                           string? Seccion = null, string Action = "Index", RolUsuario[]? Roles = null)
    {
        public bool PermitidoPara(ClaimsPrincipal user) =>
            Roles is null || Roles.Length == 0 || Roles.Any(r => user.IsInRole(r.ToString()));
    }

    public static class Menu
    {
        private static readonly RolUsuario Admin = RolUsuario.Administrador;
        private static readonly RolUsuario Jefe = RolUsuario.JefeAlmacen;
        private static readonly RolUsuario Recep = RolUsuario.Recepcionista;
        private static readonly RolUsuario Mec = RolUsuario.Mecanico;

        public static readonly IReadOnlyList<MenuItem> Items =
        [
            new("Panel",                "Home",           "panel",          Construido: true),
            new("Órdenes de trabajo",   "Ordenes",        "ordenes",        Construido: false, Roles: [Admin, Recep, Mec, Jefe]),
            new("Clientes y vehículos", "Clientes",       "clientes",       Construido: false, Roles: [Admin, Recep, Mec, Jefe]),
            new("Stock por taller",     "Stock",          "stock",          Construido: false),
            new("Inventario por sedes", "Inventario",     "inventario",     Construido: false, Roles: [Admin, Jefe]),
            new("Kardex",               "Kardex",         "kardex",         Construido: false, Roles: [Admin, Jefe]),
            new("Transferencias",       "Transferencias", "transferencias", Construido: false, Roles: [Admin, Jefe, Mec]),
            new("Compras",              "Compras",        "compras",        Construido: false, Roles: [Admin, Jefe]),
            new("Toma física",          "TomaFisica",     "toma",           Construido: false, Roles: [Admin, Jefe]),
            new("Usuarios",             "Usuarios",       "usuario",        Construido: false, Seccion: "Administración", Roles: [Admin]),
            new("Auditoría",            "Auditoria",      "auditoria",      Construido: false, Seccion: "Administración", Roles: [Admin]),
        ];

        public static IEnumerable<MenuItem> ParaUsuario(ClaimsPrincipal user) => Items.Where(i => i.PermitidoPara(user));

        public static bool EstaConstruido(string controller) =>
            Items.Any(i => i.Construido && string.Equals(i.Controller, controller, StringComparison.OrdinalIgnoreCase));
    }
}
