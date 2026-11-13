using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAutomotriz.Models;
using System.Text.Json;
using Newtonsoft.Json;
using SistemaAutomotriz.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaAutomotriz.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly AppDbContext _context;

        public ProveedorController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(string? Busqueda)
        {

            var query = _context.Proveedores.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                query.Where(p => p.RazonSocial.Contains(Busqueda) || p.Contacto.Contains(Busqueda));
            }

            var listado = query.ToList();

            return View(listado);
        }

        [Authorize]
        [HttpPost]
        public IActionResult GestionarProveedor(Proveedor pr)
        {
            if (pr.Id == 0)
            {
                _context.Proveedores.Add(pr);
            }
            else
            {
                _context.Proveedores.Update(pr);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Proveedores");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Detalle(int id)
        {
            var proveedor = _context.Proveedores.FirstOrDefault(p => p.Id == id);

            if(proveedor == null)
            {
                return Json($"El {id} ingresado no es valido, Intente de nuevo");
            }

            return Json(proveedor);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            var proveedor = _context.Proveedores.FirstOrDefault(p => p.Id == id);

            if (proveedor == null)
            {
                return Json($"El {id} ingresado no es valido, Intente de nuevo");
            }

            _context.Proveedores.Where(p => p.Id == id).ExecuteDelete();

            return RedirectToAction("Index", "Proveedores");
        }
    }
}
