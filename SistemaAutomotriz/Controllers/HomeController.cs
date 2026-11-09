using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaAutomotriz.Helpers;
using SistemaAutomotriz.Models;
using SistemaAutomotriz.Models.Enums;
using SistemaAutomotriz.Servicios;
using SistemaAutomotriz.ViewModels;

namespace SistemaAutomotriz.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataAccessService _dataAccess;

        public HomeController(DataAccessService dataAccess) => _dataAccess = dataAccess;

        [Authorize]
        public async Task<IActionResult> Index(int? sucursalId)
        {
            var esAdmin = User.IsInRole(nameof(RolUsuario.Administrador));
            var miSucursal = User.ObtenerSucursalId() ?? 0;
            var sucursales = await _dataAccess.Sucursal.ListarActivasAsync();

            var idSede = esAdmin && sucursalId is not null && sucursales.Any(s => s.Id == sucursalId)
                ? sucursalId.Value
                : miSucursal;

            var ahora = Fechas.AhoraLocal();
            var nombre = User.Identity?.Name ?? "";
            var vm = new PanelViewModel
            {
                Saludo = ahora.Hour < 12 ? "Buen día" : ahora.Hour < 19 ? "Buenas tardes" : "Buenas noches",
                PrimerNombre = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? nombre,
                FechaTexto = ahora.ToString("dddd d 'de' MMMM 'de' yyyy", Fechas.Cultura),
                SucursalId = idSede,
                SucursalNombre = sucursales.FirstOrDefault(s => s.Id == idSede)?.Nombre ?? User.ObtenerSucursalNombre(),
                PuedeCambiarSucursal = esAdmin && sucursales.Count > 1,
                Sucursales = sucursales.Select(s => new SelectListItem(s.Nombre, s.Id.ToString(), s.Id == idSede))
            };

            await _dataAccess.Panel.CargarAsync(vm, idSede);
            return View(vm);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
