using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaAutomotriz.Helpers;
using SistemaAutomotriz.Servicios;
using SistemaAutomotriz.Servicios.DA;
using SistemaAutomotriz.ViewModels;

namespace SistemaAutomotriz.Controllers
{
    public class CuentaController : Controller
    {
        private readonly DataAccessService _dataAccess;

        public CuentaController(DataAccessService dataAccess) => _dataAccess = dataAccess;

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View(model);

            var (resultado, usuario) = await _dataAccess.Usuario.ValidarCredencialesAsync(model.Correo, model.Clave);

            if (resultado == ResultadoLogin.Desactivado)
            {
                ModelState.AddModelError(string.Empty, "Tu cuenta está desactivada");
                return View(model);
            }
            if (resultado != ResultadoLogin.Ok || usuario is null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nombre),
                new(ClaimTypes.Email, usuario.Correo),
                new(ClaimTypes.Role, usuario.Rol.ToString()),
                new(ClaimsUsuario.SucursalId, usuario.SucursalId.ToString()),
                new(ClaimsUsuario.SucursalNombre, usuario.Sucursal.Nombre)
            };
            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidad),
                new AuthenticationProperties { IsPersistent = model.Recordarme });

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Registro()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            var model = new RegistroViewModel();
            await CargarListasAsync(model);
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");

            if (model.Rol is not null && !RolHelper.RolesRegistro.Contains(model.Rol.Value))
                ModelState.AddModelError(nameof(model.Rol), "Rol no permitido");

            if (model.SucursalId is not null && !await _dataAccess.Sucursal.EsActivaAsync(model.SucursalId.Value))
                ModelState.AddModelError(nameof(model.SucursalId), "Sucursal no válida");

            if (!string.IsNullOrWhiteSpace(model.Correo) && await _dataAccess.Usuario.ExisteCorreoAsync(model.Correo))
                ModelState.AddModelError(nameof(model.Correo), "Este correo ya está registrado");

            if (!ModelState.IsValid)
            {
                await CargarListasAsync(model);
                return View(model);
            }

            await _dataAccess.Usuario.RegistrarAsync(model.Nombre, model.Correo, model.Clave, model.Rol!.Value, model.SucursalId!.Value);

            TempData["Exito"] = "Cuenta creada. Ya puedes iniciar sesión";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet, AllowAnonymous]
        public IActionResult AccesoDenegado() => View();

        private async Task CargarListasAsync(RegistroViewModel model)
        {
            var sucursales = await _dataAccess.Sucursal.ListarActivasAsync();
            model.Sucursales = sucursales.Select(s => new SelectListItem(s.Nombre, s.Id.ToString()));
            model.Roles = RolHelper.RolesRegistro.Select(r => new SelectListItem(RolHelper.Texto(r), r.ToString()));
        }
    }
}
