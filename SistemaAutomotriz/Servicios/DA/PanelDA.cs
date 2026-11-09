using Microsoft.EntityFrameworkCore;
using SistemaAutomotriz.Data;
using SistemaAutomotriz.Helpers;
using SistemaAutomotriz.Models.Enums;
using SistemaAutomotriz.ViewModels;

namespace SistemaAutomotriz.Servicios.DA
{
    public class PanelDA
    {
        private const int MaxAtencion = 5;
        private const int MaxAlertasStock = 5;
        private const int MaxOrdenesRecientes = 5;
        private const int MaxRepuestosUsados = 4;

        private readonly AppDbContext _db;

        public PanelDA(AppDbContext db) => _db = db;

        public async Task CargarAsync(PanelViewModel vm, int sucursalId)
        {
            var hoyLocal = Fechas.AhoraLocal().Date;
            var hoyUtc = Fechas.InicioDiaUtc(hoyLocal);
            var semanaUtc = Fechas.InicioDiaUtc(hoyLocal.AddDays(-6));
            var mesUtc = Fechas.InicioDiaUtc(new DateTime(hoyLocal.Year, hoyLocal.Month, 1));

            var otra = await _db.Sucursales.AsNoTracking()
                .Where(s => s.Id != sucursalId && s.Activa)
                .OrderBy(s => s.Id)
                .Select(s => new { s.Id, s.Nombre })
                .FirstOrDefaultAsync();
            var otraId = otra?.Id ?? 0;
            vm.OtraSucursalNombre = otra?.Nombre ?? "";
            var otraCorto = vm.OtraSucursalNombre.Replace("Taller ", "");

            var ordenes = _db.OrdenesTrabajo.AsNoTracking().Where(o => o.SucursalId == sucursalId);
            var enCurso = EstadoOrdenHelper.EnCurso;

            var totalEnCurso = await ordenes.CountAsync(o => enCurso.Contains(o.Estado));
            var ingresaronHoy = await ordenes.CountAsync(o => enCurso.Contains(o.Estado) && o.Fecha >= hoyUtc);
            var listas = await ordenes.CountAsync(o => o.Estado == EstadoOrden.Finalizada);

            var comprobantesHoy = _db.Comprobantes.AsNoTracking()
                .Where(c => c.OrdenTrabajo.SucursalId == sucursalId && c.Fecha >= hoyUtc);
            var facturadoHoy = await comprobantesHoy.SumAsync(c => (decimal?)c.Total) ?? 0;
            var cantComprobantesHoy = await comprobantesHoy.CountAsync();

            var bajoMinimo = await _db.StockSucursales.AsNoTracking()
                .Where(s => s.SucursalId == sucursalId && s.Cantidad < s.Repuesto.StockMinimo)
                .Select(s => new
                {
                    s.Repuesto.Nombre,
                    s.Cantidad,
                    Minimo = s.Repuesto.StockMinimo,
                    Otra = s.Repuesto.Stocks.Where(o => o.SucursalId == otraId).Select(o => (int?)o.Cantidad).FirstOrDefault() ?? 0,
                    OtraTransito = s.Repuesto.Stocks.Where(o => o.SucursalId == otraId).Select(o => (int?)o.EnTransito).FirstOrDefault() ?? 0
                })
                .OrderBy(s => s.Cantidad * 1.0 / (s.Minimo == 0 ? 1 : s.Minimo))
                .ToListAsync();

            var pendientes = _db.Transferencias.AsNoTracking();
            var porAprobar = await pendientes.CountAsync(t => t.SucursalOrigenId == sucursalId && t.Estado == EstadoTransferencia.Solicitada);
            var porDespachar = await pendientes.CountAsync(t => t.SucursalOrigenId == sucursalId && t.Estado == EstadoTransferencia.Aprobada);
            var porRecibir = await pendientes.CountAsync(t => t.SucursalDestinoId == sucursalId && t.Estado == EstadoTransferencia.Despachada);

            vm.Kpis =
            [
                new("Órdenes en curso", totalEnCurso.ToString(), $"{ingresaronHoy} {(ingresaronHoy == 1 ? "ingresó" : "ingresaron")} hoy", "ordenes"),
                new("Listas para entregar", listas.ToString(), "Finalizadas sin entregar", "check", "exito"),
                new("Facturado hoy", Soles(facturadoHoy, 0), $"{cantComprobantesHoy} {(cantComprobantesHoy == 1 ? "comprobante simulado" : "comprobantes simulados")}", "descarga"),
                new("Repuestos bajo mínimo", bajoMinimo.Count.ToString(),
                    otraCorto == "" ? "Stock actual menor al mínimo" : $"{bajoMinimo.Count(b => b.Otra > 0)} con stock en {vm.OtraSucursalNombre}", "alerta", "error"),
                new("Transferencias pendientes", (porAprobar + porDespachar + porRecibir).ToString(),
                    $"{porAprobar} por aprobar · {porDespachar} por despachar", "transferencias", "aviso"),
            ];

            var porEstado = await ordenes
                .Where(o => o.Estado != EstadoOrden.Entregada)
                .GroupBy(o => o.Estado)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToListAsync();
            vm.OrdenesPorEstado = enCurso.Append(EstadoOrden.Finalizada)
                .Select(e => new BarraItem(EstadoOrdenHelper.Texto(e),
                    porEstado.FirstOrDefault(p => p.Estado == e)?.Cantidad ?? 0,
                    EstadoOrdenHelper.ClaseBarra(e)))
                .ToList();

            var carga = await ordenes
                .Where(o => enCurso.Contains(o.Estado))
                .GroupBy(o => new { o.MecanicoId, Nombre = o.Mecanico != null ? o.Mecanico.Nombre : null })
                .Select(g => new { g.Key.MecanicoId, g.Key.Nombre, Cantidad = g.Count() })
                .ToListAsync();
            vm.CargaMecanicos = carga.Where(c => c.MecanicoId != null)
                .OrderByDescending(c => c.Cantidad)
                .Select(c => new BarraItem(c.Nombre ?? "", c.Cantidad))
                .ToList();
            var sinAsignar = carga.Where(c => c.MecanicoId == null).Sum(c => c.Cantidad);
            if (sinAsignar > 0) vm.CargaMecanicos.Add(new BarraItem("Sin asignar", sinAsignar, "c-rojo"));

            var comprobantesSemana = await _db.Comprobantes.AsNoTracking()
                .Where(c => c.OrdenTrabajo.SucursalId == sucursalId && c.Fecha >= semanaUtc)
                .Select(c => new { c.Fecha, c.Total })
                .ToListAsync();
            vm.FacturadoSemana = Enumerable.Range(0, 7)
                .Select(i => hoyLocal.AddDays(i - 6))
                .Select(dia => new DiaFacturado(
                    Fechas.Capitalizar(dia.ToString("ddd", Fechas.Cultura).TrimEnd('.')),
                    comprobantesSemana.Where(c => Fechas.ALocal(c.Fecha).Date == dia).Sum(c => c.Total),
                    dia == hoyLocal))
                .ToList();
            vm.TotalSemana = vm.FacturadoSemana.Sum(d => d.Monto);

            var finalizadasAt = new List<AtencionItem>();
            var cotizadasAt = new List<AtencionItem>();
            var sinMecanicoAt = new List<AtencionItem>();
            var esperaAt = new List<AtencionItem>();

            var finalizadas = await ordenes
                .Where(o => o.Estado == EstadoOrden.Finalizada)
                .OrderBy(o => o.Fecha)
                .Select(o => new { o.Id, o.Vehiculo.Placa, o.Fecha, TieneCorreo = o.Vehiculo.Cliente.Correo != null })
                .ToListAsync();
            finalizadasAt.AddRange(finalizadas.Select(o => new AtencionItem(
                $"{NumeroOrden(o.Id)} · {o.Placa} finalizada sin entregar",
                $"Ingresó {Hace(o.Fecha, hoyLocal)}" + (o.TieneCorreo ? " · avisar al cliente por correo" : " · avisar al cliente"),
                "error", new BotonModulo("Ver orden", "Ordenes", "Detalle", o.Id))));

            var cotizadas = await ordenes
                .Where(o => o.Estado == EstadoOrden.Cotizada)
                .OrderBy(o => o.Fecha)
                .Select(o => new { o.Id, o.Vehiculo.Placa, o.Fecha, Cliente = o.Vehiculo.Cliente.Nombre })
                .ToListAsync();
            cotizadasAt.AddRange(cotizadas.Select(o => new AtencionItem(
                $"{NumeroOrden(o.Id)} · {o.Placa} cotización sin respuesta",
                $"Ingresó {Hace(o.Fecha, hoyLocal)} · {o.Cliente}",
                "aviso", new BotonModulo("Ver orden", "Ordenes", "Detalle", o.Id))));

            var sinMecanico = await ordenes
                .Where(o => enCurso.Contains(o.Estado) && o.MecanicoId == null)
                .OrderBy(o => o.Fecha)
                .Select(o => new { o.Id, o.Vehiculo.Placa, o.Fecha })
                .ToListAsync();
            sinMecanicoAt.AddRange(sinMecanico.Select(o => new AtencionItem(
                $"{NumeroOrden(o.Id)} · {o.Placa} sin mecánico",
                $"Recibida {Hace(o.Fecha, hoyLocal)} · asignar responsable",
                "aviso", new BotonModulo("Asignar", "Ordenes", "Detalle", o.Id))));

            var esperaRepuesto = await _db.OrdenRepuestos.AsNoTracking()
                .Where(r => r.OrdenTrabajo.SucursalId == sucursalId
                         && (r.OrdenTrabajo.Estado == EstadoOrden.Aprobada || r.OrdenTrabajo.Estado == EstadoOrden.EnReparacion))
                .Select(r => new
                {
                    r.OrdenTrabajoId,
                    r.OrdenTrabajo.Vehiculo.Placa,
                    Repuesto = r.Repuesto.Nombre,
                    r.Repuesto.StockMinimo,
                    Stock = r.Repuesto.Stocks.Where(s => s.SucursalId == sucursalId).Select(s => (int?)s.Cantidad).FirstOrDefault() ?? 0
                })
                .Where(r => r.Stock < r.StockMinimo)
                .ToListAsync();
            esperaAt.AddRange(esperaRepuesto
                .GroupBy(r => r.OrdenTrabajoId)
                .Select(g => g.First())
                .Select(r => new AtencionItem(
                    $"{NumeroOrden(r.OrdenTrabajoId)} · {r.Placa} espera repuesto",
                    $"Falta {r.Repuesto} (stock {r.Stock})",
                    "error", new BotonModulo("Solicitar", "Transferencias", "Nueva"))));

            var atencion = Intercalar(finalizadasAt, cotizadasAt, sinMecanicoAt, esperaAt);
            vm.TotalAtencion = atencion.Count;
            vm.Atencion = atencion.Take(MaxAtencion).ToList();

            vm.AlertasStock = bajoMinimo.Take(MaxAlertasStock)
                .Select(b => new AlertaStockItem(b.Nombre, b.Cantidad, b.Minimo, b.Otra, b.OtraTransito))
                .ToList();

            vm.OrdenesRecientes = await ordenes
                .OrderByDescending(o => o.Id)
                .Take(MaxOrdenesRecientes)
                .Select(o => new OrdenRecienteItem(
                    o.Id,
                    o.Vehiculo.Placa,
                    o.Vehiculo.Marca + " " + o.Vehiculo.Modelo,
                    o.Vehiculo.Cliente.Nombre,
                    o.Estado,
                    o.Mecanico != null ? o.Mecanico.Nombre : "Sin asignar",
                    (o.Servicios.Sum(s => (decimal?)s.Precio) ?? 0)
                        + (o.Repuestos.Sum(r => (decimal?)(r.Cantidad * r.PrecioUnitario)) ?? 0)))
                .ToListAsync();

            var transferencias = await _db.Transferencias.AsNoTracking()
                .Where(t => (t.SucursalOrigenId == sucursalId &&
                             (t.Estado == EstadoTransferencia.Solicitada || t.Estado == EstadoTransferencia.Aprobada))
                         || (t.SucursalDestinoId == sucursalId && t.Estado == EstadoTransferencia.Despachada))
                .OrderByDescending(t => t.Id)
                .Select(t => new
                {
                    t.Id,
                    t.Estado,
                    Origen = t.SucursalOrigen.Nombre,
                    Destino = t.SucursalDestino.Nombre,
                    Primero = t.Detalles.OrderBy(d => d.Id).Select(d => d.Repuesto.Nombre + " x " + d.Cantidad).FirstOrDefault(),
                    Items = t.Detalles.Count()
                })
                .ToListAsync();
            vm.Transferencias = transferencias.Select(t =>
            {
                var titulo = $"T-{t.Id:0000} · {t.Primero}" + (t.Items > 1 ? $" (+{t.Items - 1} más)" : "");
                return t.Estado switch
                {
                    EstadoTransferencia.Solicitada => new TransferenciaPendienteItem(titulo, $"Solicitada por {t.Destino}",
                        new BotonModulo("Revisar", "Transferencias", "Detalle", t.Id)),
                    EstadoTransferencia.Aprobada => new TransferenciaPendienteItem(titulo, "Aprobada, lista para despachar",
                        new BotonModulo("Despachar", "Transferencias", "Detalle", t.Id)),
                    _ => new TransferenciaPendienteItem(titulo, $"En tránsito desde {t.Origen}",
                        new BotonModulo("Recibir", "Transferencias", "Detalle", t.Id)),
                };
            }).ToList();

            vm.MesActual = Fechas.Capitalizar(hoyLocal.ToString("MMMM", Fechas.Cultura));
            var usados = await _db.MovimientosKardex.AsNoTracking()
                .Where(k => k.SucursalId == sucursalId && k.Tipo == TipoMovimiento.SalidaOrden && k.Fecha >= mesUtc)
                .GroupBy(k => k.Repuesto.Nombre)
                .Select(g => new { Nombre = g.Key, Cantidad = g.Sum(k => k.Cantidad < 0 ? -k.Cantidad : k.Cantidad) })
                .OrderByDescending(g => g.Cantidad)
                .Take(MaxRepuestosUsados)
                .ToListAsync();
            vm.RepuestosMasUsados = usados.Select(u => new BarraItem(u.Nombre, u.Cantidad, "c-celeste")).ToList();
        }

        private static List<T> Intercalar<T>(params List<T>[] listas)
        {
            var resultado = new List<T>();
            for (var i = 0; i < listas.Max(l => l.Count); i++)
                foreach (var l in listas)
                    if (i < l.Count) resultado.Add(l[i]);
            return resultado;
        }

        public static string NumeroOrden(int id) => $"#{id:0000}";

        public static string Soles(decimal monto, int decimales = 2) =>
            "S/ " + monto.ToString(decimales == 0 ? "#,0" : "#,0.00", System.Globalization.CultureInfo.InvariantCulture);

        private static string Hace(DateTime fechaUtc, DateTime hoyLocal)
        {
            var dias = (hoyLocal - Fechas.ALocal(fechaUtc).Date).Days;
            return dias switch
            {
                <= 0 => "hoy",
                1 => "ayer",
                _ => $"hace {dias} días"
            };
        }
    }
}
