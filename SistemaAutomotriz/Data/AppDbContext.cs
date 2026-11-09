using Microsoft.EntityFrameworkCore;
using SistemaAutomotriz.Models;

namespace SistemaAutomotriz.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Sucursal> Sucursales => Set<Sucursal>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Auditoria> Auditorias => Set<Auditoria>();
        public DbSet<TokenAcceso> TokensAcceso => Set<TokenAcceso>();

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
        public DbSet<Servicio> Servicios => Set<Servicio>();
        public DbSet<OrdenTrabajo> OrdenesTrabajo => Set<OrdenTrabajo>();
        public DbSet<OrdenServicio> OrdenServicios => Set<OrdenServicio>();
        public DbSet<OrdenRepuesto> OrdenRepuestos => Set<OrdenRepuesto>();
        public DbSet<Comprobante> Comprobantes => Set<Comprobante>();

        public DbSet<Proveedor> Proveedores => Set<Proveedor>();
        public DbSet<Repuesto> Repuestos => Set<Repuesto>();
        public DbSet<StockSucursal> StockSucursales => Set<StockSucursal>();
        public DbSet<MovimientoKardex> MovimientosKardex => Set<MovimientoKardex>();
        public DbSet<Transferencia> Transferencias => Set<Transferencia>();
        public DbSet<TransferenciaDetalle> TransferenciaDetalles => Set<TransferenciaDetalle>();
        public DbSet<OrdenCompra> OrdenesCompra => Set<OrdenCompra>();
        public DbSet<OrdenCompraDetalle> OrdenCompraDetalles => Set<OrdenCompraDetalle>();
        public DbSet<TomaFisica> TomasFisicas => Set<TomaFisica>();
        public DbSet<TomaFisicaDetalle> TomaFisicaDetalles => Set<TomaFisicaDetalle>();

        protected override void ConfigureConventions(ModelConfigurationBuilder c)
        {
            c.Properties<decimal>().HavePrecision(10, 2);
            c.Properties<Enum>().HaveConversion<string>().HaveMaxLength(30);
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Sucursal>(e =>
            {
                e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                e.Property(x => x.Direccion).HasMaxLength(200);
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            mb.Entity<Usuario>(e =>
            {
                e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                e.Property(x => x.Correo).HasMaxLength(150).IsRequired();
                e.Property(x => x.ClaveHash).HasMaxLength(500).IsRequired();
                e.HasIndex(x => x.Correo).IsUnique();
                e.HasOne(x => x.Sucursal).WithMany(s => s.Usuarios).HasForeignKey(x => x.SucursalId);
            });

            mb.Entity<Cliente>(e =>
            {
                e.Property(x => x.Documento).HasMaxLength(15).IsRequired();
                e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
                e.Property(x => x.Telefono).HasMaxLength(20);
                e.Property(x => x.Correo).HasMaxLength(150);
                e.HasIndex(x => x.Documento).IsUnique();
            });

            mb.Entity<Vehiculo>(e =>
            {
                e.Property(x => x.Placa).HasMaxLength(10).IsRequired();
                e.Property(x => x.Marca).HasMaxLength(50).IsRequired();
                e.Property(x => x.Modelo).HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Placa).IsUnique();
                e.HasOne(x => x.Cliente).WithMany(c => c.Vehiculos).HasForeignKey(x => x.ClienteId);
            });

            mb.Entity<Servicio>(e =>
            {
                e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                e.ToTable(t => t.HasCheckConstraint("CK_Servicios_PrecioBase", "[PrecioBase] >= 0"));
            });

            mb.Entity<OrdenTrabajo>(e =>
            {
                e.Property(x => x.Diagnostico).HasMaxLength(1000);
                e.HasOne(x => x.Vehiculo).WithMany(v => v.Ordenes).HasForeignKey(x => x.VehiculoId);
                e.HasOne(x => x.Sucursal).WithMany().HasForeignKey(x => x.SucursalId);
                e.HasOne(x => x.Mecanico).WithMany().HasForeignKey(x => x.MecanicoId);
                e.HasIndex(x => new { x.SucursalId, x.Estado });
            });

            mb.Entity<OrdenServicio>(e =>
            {
                e.HasOne(x => x.OrdenTrabajo).WithMany(o => o.Servicios)
                    .HasForeignKey(x => x.OrdenTrabajoId).OnDelete(DeleteBehavior.Cascade);
            });

            mb.Entity<OrdenRepuesto>(e =>
            {
                e.HasOne(x => x.OrdenTrabajo).WithMany(o => o.Repuestos)
                    .HasForeignKey(x => x.OrdenTrabajoId).OnDelete(DeleteBehavior.Cascade);
                e.ToTable(t => t.HasCheckConstraint("CK_OrdenRepuestos_Cantidad", "[Cantidad] > 0"));
            });

            mb.Entity<Comprobante>(e =>
            {
                e.Property(x => x.Serie).HasMaxLength(4).IsRequired();
                e.HasIndex(x => new { x.Serie, x.Correlativo }).IsUnique();
                e.HasIndex(x => x.OrdenTrabajoId).IsUnique();
            });

            mb.Entity<Proveedor>(e =>
            {
                e.Property(x => x.Ruc).HasMaxLength(11).IsRequired();
                e.Property(x => x.RazonSocial).HasMaxLength(150).IsRequired();
                e.Property(x => x.Contacto).HasMaxLength(150);
                e.HasIndex(x => x.Ruc).IsUnique();
            });

            mb.Entity<Repuesto>(e =>
            {
                e.Property(x => x.Codigo).HasMaxLength(30).IsRequired();
                e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
                e.Property(x => x.Categoria).HasMaxLength(60);
                e.HasIndex(x => x.Codigo).IsUnique();
                e.ToTable(t => t.HasCheckConstraint("CK_Repuestos_StockMinimo", "[StockMinimo] >= 0"));
            });

            mb.Entity<StockSucursal>(e =>
            {
                e.HasKey(x => new { x.RepuestoId, x.SucursalId });
                e.HasOne(x => x.Repuesto).WithMany(r => r.Stocks).HasForeignKey(x => x.RepuestoId);
                e.HasOne(x => x.Sucursal).WithMany(s => s.Stocks).HasForeignKey(x => x.SucursalId);
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_StockSucursales_Cantidad", "[Cantidad] >= 0");
                    t.HasCheckConstraint("CK_StockSucursales_EnTransito", "[EnTransito] >= 0");
                });
            });

            mb.Entity<MovimientoKardex>(e =>
            {
                e.Property(x => x.Referencia).HasMaxLength(100);
                e.HasIndex(x => new { x.RepuestoId, x.SucursalId, x.Fecha });
            });

            mb.Entity<Transferencia>(e =>
            {
                e.HasOne(x => x.SucursalOrigen).WithMany().HasForeignKey(x => x.SucursalOrigenId);
                e.HasOne(x => x.SucursalDestino).WithMany().HasForeignKey(x => x.SucursalDestinoId);
                e.HasOne(x => x.Solicitante).WithMany().HasForeignKey(x => x.SolicitanteId);
                e.ToTable(t => t.HasCheckConstraint("CK_Transferencias_Sucursales",
                    "[SucursalOrigenId] <> [SucursalDestinoId]"));
            });

            mb.Entity<TransferenciaDetalle>(e =>
            {
                e.HasOne(x => x.Transferencia).WithMany(t => t.Detalles)
                    .HasForeignKey(x => x.TransferenciaId).OnDelete(DeleteBehavior.Cascade);
                e.ToTable(t => t.HasCheckConstraint("CK_TransferenciaDetalles_Cantidad", "[Cantidad] > 0"));
            });

            mb.Entity<OrdenCompraDetalle>(e =>
            {
                e.HasOne(x => x.OrdenCompra).WithMany(o => o.Detalles)
                    .HasForeignKey(x => x.OrdenCompraId).OnDelete(DeleteBehavior.Cascade);
                e.ToTable(t => t.HasCheckConstraint("CK_OrdenCompraDetalles_Cantidad", "[Cantidad] > 0"));
            });

            mb.Entity<TomaFisicaDetalle>(e =>
            {
                e.HasOne(x => x.TomaFisica).WithMany(t => t.Detalles)
                    .HasForeignKey(x => x.TomaFisicaId).OnDelete(DeleteBehavior.Cascade);
            });

            mb.Entity<Auditoria>(e =>
            {
                e.ToTable("Auditoria");
                e.Property(x => x.Accion).HasMaxLength(50).IsRequired();
                e.Property(x => x.Entidad).HasMaxLength(60).IsRequired();
                e.Property(x => x.EntidadId).HasMaxLength(30);
                e.Property(x => x.Detalle).HasMaxLength(1000);
                e.HasIndex(x => x.Fecha);
            });

            mb.Entity<TokenAcceso>(e =>
            {
                e.Property(x => x.Token).HasMaxLength(100).IsRequired();
                e.HasIndex(x => x.Token).IsUnique();
            });

            var cascadas = new HashSet<(Type Dependiente, Type Principal)>
            {
                (typeof(OrdenServicio), typeof(OrdenTrabajo)),
                (typeof(OrdenRepuesto), typeof(OrdenTrabajo)),
                (typeof(TransferenciaDetalle), typeof(Transferencia)),
                (typeof(OrdenCompraDetalle), typeof(OrdenCompra)),
                (typeof(TomaFisicaDetalle), typeof(TomaFisica)),
            };
            foreach (var fk in mb.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
            {
                if (!cascadas.Contains((fk.DeclaringEntityType.ClrType, fk.PrincipalEntityType.ClrType)))
                    fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
