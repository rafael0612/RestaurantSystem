using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Common;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.Infrastructure.Persistence.Repositories
{
    public class CajaRepository : ICajaRepository
    {
        private readonly RestaurantSystemDbContext _db;
        public CajaRepository(RestaurantSystemDbContext db) => _db = db;

        public Task<CajaSesion?> GetCajaAbiertaAsync(CancellationToken ct)
            => _db.CajasSesion
                .OrderByDescending(x => x.AperturaEn)
                .FirstOrDefaultAsync(x => x.Estado == EstadoCajaSesion.Abierta, ct);

        public Task<CajaSesion?> GetSesionAbiertaPorUsuarioAsync(Guid UserId, CancellationToken ct)
            => _db.CajasSesion.AsNoTracking()
                .Where(x => x.UsuarioId == UserId && x.Estado == EstadoCajaSesion.Abierta)
                .OrderByDescending(x => x.AperturaEn)
                .FirstOrDefaultAsync(ct);

        public async Task<CajaSesionDto?> GetSesionAbiertaDtoAsync(CancellationToken ct)
        {
            var ses = await GetCajaAbiertaAsync(ct);
            if (ses is null) return null;

            return new CajaSesionDto(
                CajaSesionId: ses.Id,
                AperturaEn: ses.AperturaEn,
                MontoApertura: ses.MontoApertura,
                Abierta: ses.Estado == EstadoCajaSesion.Abierta,
                CierreEn: ses.CierreEn,
                MontoCierreContado: ses.MontoCierreContado,
                EfectivoEsperado: ses.EfectivoEsperado,
                Diferencia: ses.Diferencia
            );
        }

        public Task AddCajaSesionAsync(CajaSesion sesion, CancellationToken ct)
        {
            _db.CajasSesion.Add(sesion);
            return Task.CompletedTask;
        }

        public Task AddPagoAsync(Pago pago, CancellationToken ct)
        {
            _db.Pagos.Add(pago);
            return Task.CompletedTask;
        }

        public Task AddMovimientoAsync(MovimientoCaja mov, CancellationToken ct)
        {
            _db.MovimientosCaja.Add(mov);
            return Task.CompletedTask;
        }

        public async Task<decimal> CalcularEfectivoEsperadoAsync(Guid cajaSesionId, CancellationToken ct)
        {
            // Ingresos en efectivo = suma de PagoMetodo(Efectivo) de pagos no anulados de esa sesión
            //var ingresosEfectivo = await _db.PagoMetodos
            //    .Where(pm => pm.Metodo == MetodoPago.Efectivo
            //                 && _db.Pagos.Any(p => p.Id == pm.PagoId && !p.Anulado && p.CajaSesionId == cajaSesionId))
            //    .SumAsync(pm => (decimal?)pm.Monto, ct) ?? 0m;

            var ingresosEfectivo = await (
                                        from pm in _db.PagoMetodos.AsNoTracking()
                                        join p in _db.Pagos.AsNoTracking() on pm.PagoId equals p.Id
                                        where p.CajaSesionId == cajaSesionId && !p.Anulado && pm.Metodo == MetodoPago.Efectivo
                                        select (decimal?)pm.Monto
                                    ).SumAsync(ct) ?? 0m;

            // Egresos = suma MovimientoCaja(Egreso) de esa sesión
            var egresos = await _db.MovimientosCaja.AsNoTracking()
                .Where(m => m.CajaSesionId == cajaSesionId && m.Tipo == TipoMovimientoCaja.Egreso)
                .SumAsync(m => (decimal?)m.Monto, ct) ?? 0m;

            var apertura = await _db.CajasSesion.AsNoTracking()
                .Where(c => c.Id == cajaSesionId)
                .Select(c => c.MontoApertura)
                .FirstAsync(ct);

            return apertura + ingresosEfectivo - egresos;
        }
        public async Task<List<CuentaPorCobrarDto>> ListarCuentasPorCobrarAsync(CancellationToken ct)
        {
            // Incluimos lo necesario para calcular totales por propiedades de dominio (en memoria)
            var cuentas = await _db.Cuentas
                .Include(c => c.Comandas).ThenInclude(cmd => cmd.Detalles)
                .Include(c => c.Pagos)
                .Where(c => c.Estado == EstadoCuenta.PorCobrar)
                .OrderBy(c => c.AperturaEn)
                .ToListAsync(ct);

            var mesaIds = cuentas.Where(c => c.MesaId != null).Select(c => c.MesaId!.Value).Distinct().ToList();
            var mesaMap = await _db.Mesas.AsNoTracking()
                .Where(m => mesaIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, m => m.Nombre, ct);

            return cuentas.Select(c =>
            {
                var origen = c.MesaId is null 
                    ? "Para llevar" 
                    : (mesaMap.TryGetValue(c.MesaId.Value, out var n) ? n : "Mesa");
                return new CuentaPorCobrarDto(
                    c.Id,
                    origen,
                    c.AperturaEn,
                    c.TotalConsumido,
                    c.TotalPagado,
                    c.SaldoPendiente
                );
            }).ToList();
        }

        public async Task<CuentaCobroDto> GetCuentaCobroAsync(Guid cuentaId, CancellationToken ct)
        {
            var cuenta = await _db.Cuentas
                .Include(c => c.Comandas).ThenInclude(cmd => cmd.Detalles)
                .Include(c => c.Pagos).ThenInclude(p => p.Detalles)
                .FirstOrDefaultAsync(c => c.Id == cuentaId, ct)
                ?? throw new KeyNotFoundException("Cuenta no existe.");

            string origen;
            if (cuenta.MesaId is null)
            {
                origen = "Para llevar";
            }
            else
            {
                origen = await _db.Mesas.AsNoTracking()
                    .Where(m => m.Id == cuenta.MesaId.Value)
                    .Select(m => m.Nombre)
                    .FirstOrDefaultAsync(ct) ?? "Mesa";
            }

            // Items pendientes: cantidad pendiente > 0 y no anulado
            var items = (
                from cmd in cuenta.Comandas
                from det in cmd.Detalles
                where !det.Anulado && det.CantidadPendientePago > 0
                select new CobroItemDto(
                    ComandaDetalleId: det.Id,
                    ComandaNumero: cmd.NumeroSecuencia,
                    Producto: "",              // lo resolvemos con join a Producto
                    CantidadPendiente: det.CantidadPendientePago,
                    PrecioUnitario: det.PrecioUnitario,
                    Observacion: det.Observacion,
                    EstadoCocina: det.EstadoCocina.ToShared()
                )
            ).ToList();

            // Completar nombres de producto sin N+1 (diccionario)
            var prodIds = cuenta.Comandas.SelectMany(x => x.Detalles).Select(d => d.ProductoId).Distinct().ToList();
            var prodMap = await _db.Productos.AsNoTracking()
                .Where(p => prodIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Nombre, ct);

            items = items.Select(i =>
            {
                // recuperar productoId de det con lookup rápido
                var det = cuenta.Comandas.SelectMany(c => c.Detalles).First(d => d.Id == i.ComandaDetalleId);
                var nombre = prodMap.TryGetValue(det.ProductoId, out var n) ? n : "Producto";
                return i with { Producto = nombre };
            }).ToList();

            return new CuentaCobroDto(
                CuentaId: cuenta.Id,
                Origen: origen,
                AperturaEn: cuenta.AperturaEn,
                TotalConsumido: cuenta.TotalConsumido,
                TotalPagado: cuenta.TotalPagado,
                SaldoPendiente: cuenta.SaldoPendiente,
                Items: items
            );
        }

        public async Task<ReporteDiarioCajaDto> GetReporteDiarioCajaAsync(DateOnly fecha, CancellationToken ct)
        {
            var start = fecha.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(1);

            var pagos = await _db.Pagos.AsNoTracking()
                .Where(p => !p.Anulado && p.PagadoEn >= start && p.PagadoEn < end)
                .OrderByDescending(p => p.PagadoEn)
                .Select(p => new { p.Id, p.PagadoEn, p.CuentaId, p.Total })
                .ToListAsync(ct);

            var cuentaIds = pagos.Select(p => p.CuentaId).Distinct().ToList();

            var cuentas = await _db.Cuentas.AsNoTracking()
                .Where(c => cuentaIds.Contains(c.Id))
                .Select(c => new { c.Id, c.MesaId })
                .ToListAsync(ct);

            var mesaIds = cuentas.Where(c => c.MesaId != null).Select(c => c.MesaId!.Value).Distinct().ToList();
            var mesaMap = await _db.Mesas.AsNoTracking()
                .Where(m => mesaIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, m => m.Nombre, ct);

            string OrigenOf(Guid cuentaId)
            {
                var c = cuentas.First(x => x.Id == cuentaId);
                if (c.MesaId is null) return "Para llevar";
                return mesaMap.TryGetValue(c.MesaId.Value, out var n) ? n : "Mesa";
            }

            var pagosDto = pagos.Select(p => new PagoResumenCajaDto(
                PagoId: p.Id,
                PagadoEn: p.PagadoEn,
                Origen: OrigenOf(p.CuentaId),
                Total: p.Total
            )).ToList();

            var totalVentas = pagosDto.Sum(p => p.Total);

            var totalesPorMetodo = await (
                from pm in _db.PagoMetodos.AsNoTracking()
                join p in _db.Pagos.AsNoTracking() on pm.PagoId equals p.Id
                where !p.Anulado && p.PagadoEn >= start && p.PagadoEn < end
                group pm by pm.Metodo into g
                select new ReporteMetodoCajaDto(g.Key.ToShared(), g.Sum(x => x.Monto))
            ).ToListAsync(ct);

            return new ReporteDiarioCajaDto(
                Fecha: fecha,
                TotalVentas: totalVentas,
                TotalesPorMetodo: totalesPorMetodo,
                Pagos: pagosDto
            );
        }
    }
}
