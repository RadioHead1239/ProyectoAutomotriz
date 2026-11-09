using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaAutomotriz.Data;
using SistemaAutomotriz.Models;
using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Servicios.DA
{
    public enum ResultadoLogin { Ok, Invalido, Desactivado }

    public class UsuarioDA
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<Usuario> _hasher;

        public UsuarioDA(AppDbContext db, IPasswordHasher<Usuario> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public static string NormalizarCorreo(string correo) => correo.Trim().ToLowerInvariant();

        public async Task<Usuario?> ObtenerPorIdAsync(int id) =>
            await _db.Usuarios.AsNoTracking()
                .Include(u => u.Sucursal)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            var normalizado = NormalizarCorreo(correo);
            return await _db.Usuarios.AsNoTracking()
                .Include(u => u.Sucursal)
                .FirstOrDefaultAsync(u => u.Correo == normalizado);
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            var normalizado = NormalizarCorreo(correo);
            return await _db.Usuarios.AnyAsync(u => u.Correo == normalizado);
        }

        public async Task<Usuario> RegistrarAsync(string nombre, string correo, string clave, RolUsuario rol, int sucursalId)
        {
            var usuario = new Usuario
            {
                Nombre = nombre.Trim(),
                Correo = NormalizarCorreo(correo),
                Rol = rol,
                SucursalId = sucursalId,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };
            usuario.ClaveHash = _hasher.HashPassword(usuario, clave);

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return usuario;
        }

        public async Task<(ResultadoLogin Resultado, Usuario? Usuario)> ValidarCredencialesAsync(string correo, string clave)
        {
            var usuario = await ObtenerPorCorreoAsync(correo);
            if (usuario is null)
                return (ResultadoLogin.Invalido, null);

            var verificacion = _hasher.VerifyHashedPassword(usuario, usuario.ClaveHash, clave);
            if (verificacion == PasswordVerificationResult.Failed)
                return (ResultadoLogin.Invalido, null);

            if (!usuario.Activo)
                return (ResultadoLogin.Desactivado, null);

            if (verificacion == PasswordVerificationResult.SuccessRehashNeeded)
                await ActualizarHashAsync(usuario.Id, clave);

            return (ResultadoLogin.Ok, usuario);
        }

        private async Task ActualizarHashAsync(int usuarioId, string clave)
        {
            var usuario = await _db.Usuarios.FindAsync(usuarioId);
            if (usuario is null) return;
            usuario.ClaveHash = _hasher.HashPassword(usuario, clave);
            await _db.SaveChangesAsync();
        }
    }
}
