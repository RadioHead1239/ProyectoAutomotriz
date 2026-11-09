using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaAutomotriz.Models;
using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.Data
{
    public static class DbSeeder
    {
        public static async Task SembrarAsync(IServiceProvider sp, IConfiguration config, IWebHostEnvironment env)
        {
            var db = sp.GetRequiredService<AppDbContext>();
            var hasher = sp.GetRequiredService<IPasswordHasher<Usuario>>();
            var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

            if (!await db.Sucursales.AnyAsync())
            {
                db.Sucursales.AddRange(
                    new Sucursal { Nombre = "Taller Central", Direccion = "Av. Principal 123, Lima" },
                    new Sucursal { Nombre = "Taller Norte", Direccion = "Av. Los Alisos 456, Los Olivos" });
                await db.SaveChangesAsync();
                log.LogInformation("Sucursales iniciales creadas.");
            }

            if (await db.Usuarios.AnyAsync(u => u.Rol == RolUsuario.Administrador))
                return;

            var correo = config["Seed:AdminEmail"];
            var clave = config["Seed:AdminPassword"];

            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(clave))
            {
                if (!env.IsDevelopment())
                {
                    log.LogWarning("Seed:AdminEmail / Seed:AdminPassword no configurados. No se creó el administrador.");
                    return;
                }
                correo = "admin@taller.com";
                clave = "Admin123!";
            }

            var central = await db.Sucursales.OrderBy(s => s.Id).FirstAsync();
            var admin = new Usuario
            {
                Nombre = "Administrador",
                Correo = correo.Trim().ToLowerInvariant(),
                Rol = RolUsuario.Administrador,
                SucursalId = central.Id,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };
            admin.ClaveHash = hasher.HashPassword(admin, clave);

            db.Usuarios.Add(admin);
            await db.SaveChangesAsync();
            log.LogInformation("Usuario administrador creado: {Correo}", admin.Correo);
        }
    }
}
