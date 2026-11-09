using System.Security.Claims;

namespace SistemaAutomotriz.Helpers
{
    public static class ClaimsUsuario
    {
        public const string SucursalId = "SucursalId";
        public const string SucursalNombre = "SucursalNombre";

        public static int? ObtenerSucursalId(this ClaimsPrincipal user) =>
            int.TryParse(user.FindFirstValue(SucursalId), out var id) ? id : null;

        public static string ObtenerSucursalNombre(this ClaimsPrincipal user) =>
            user.FindFirstValue(SucursalNombre) ?? "";

        public static string ObtenerRolTexto(this ClaimsPrincipal user) =>
            RolHelper.Texto(user.FindFirstValue(ClaimTypes.Role));
    }
}
