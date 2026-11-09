using Microsoft.EntityFrameworkCore;
using SistemaAutomotriz.Data;
using SistemaAutomotriz.Models;

namespace SistemaAutomotriz.Servicios.DA
{
    public class SucursalDA
    {
        private readonly AppDbContext _db;

        public SucursalDA(AppDbContext db) => _db = db;

        public async Task<List<Sucursal>> ListarActivasAsync() =>
            await _db.Sucursales.AsNoTracking()
                .Where(s => s.Activa)
                .OrderBy(s => s.Nombre)
                .ToListAsync();

        public async Task<Sucursal?> ObtenerPorIdAsync(int id) =>
            await _db.Sucursales.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

        public async Task<bool> EsActivaAsync(int id) =>
            await _db.Sucursales.AnyAsync(s => s.Id == id && s.Activa);
    }
}
