using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Queries
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly RestaurantSystemDbContext _db;
        public ReporteRepository(RestaurantSystemDbContext db) => _db = db;

        public async Task<(decimal ventasTotal, decimal ventasEfectivo, decimal ventasYape, decimal ventasPlin,
                           decimal egresos, decimal costosEstimados, int cantidadPedidos,
                           List<(Guid productoId, string nombre, int cantidad, decimal monto)> topProductos)>
            GetReporteDiarioAsync(DateOnly fecha, CancellationToken ct)
        {
            // Rango del día (asumimos UTC; si quieres hora local Lima lo ajustamos en API)
            var start = fecha.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(1);

            var pagosDelDia = _db.Pagos.AsNoTracking()
                .Where(p => !p.Anulado && p.PagadoEn >= start && p.PagadoEn < end);

            var ventasTotal = await pagosDelDia.SumAsync(p => (decimal?)p.Total, ct) ?? 0m;

            // Ventas por método: sum PagoMetodo.Monto unido a Pago del día
            var metodos = await (
                from pm in _db.PagoMetodos.AsNoTracking()
                join p in _db.Pagos.AsNoTracking() on pm.PagoId equals p.Id
                where !p.Anulado && p.PagadoEn >= start && p.PagadoEn < end
                group pm by pm.Metodo into g
                select new { Metodo = g.Key, Monto = g.Sum(x => x.Monto) }
            ).ToListAsync(ct);

            decimal ventasEfectivo = metodos.FirstOrDefault(x => x.Metodo == MetodoPago.Efectivo)?.Monto ?? 0m;
            decimal ventasYape = metodos.FirstOrDefault(x => x.Metodo == MetodoPago.Yape)?.Monto ?? 0m;
            decimal ventasPlin = metodos.FirstOrDefault(x => x.Metodo == MetodoPago.Plin)?.Monto ?? 0m;

            // Egresos del día (todas sesiones)
            var egresos = await _db.MovimientosCaja.AsNoTracking()
                .Where(m => m.Tipo == TipoMovimientoCaja.Egreso && m.Fecha >= start && m.Fecha < end)
                .SumAsync(m => (decimal?)m.Monto, ct) ?? 0m;

            // Costos estimados del día: basado en lo PAGADO (PagoDetalle)
            var costosEstimados = await (
                from pd in _db.PagoDetalles.AsNoTracking()
                join p in _db.Pagos.AsNoTracking() on pd.PagoId equals p.Id
                join cd in _db.ComandaDetalles.AsNoTracking() on pd.ComandaDetalleId equals cd.Id
                where !p.Anulado && p.PagadoEn >= start && p.PagadoEn < end
                select (decimal?)pd.CantidadPagada * cd.CostoUnitarioEstandar
            ).SumAsync(ct) ?? 0m;

            // Cantidad "pedidos" => cuentas con pagos ese día
            var cantidadPedidos = await pagosDelDia.Select(p => p.CuentaId).Distinct().CountAsync(ct);

            // Top productos por monto (pagado)
            var top = await (
                from pd in _db.PagoDetalles.AsNoTracking()
                join p in _db.Pagos.AsNoTracking() on pd.PagoId equals p.Id
                join cd in _db.ComandaDetalles.AsNoTracking() on pd.ComandaDetalleId equals cd.Id
                join pr in _db.Productos.AsNoTracking() on cd.ProductoId equals pr.Id
                where !p.Anulado && p.PagadoEn >= start && p.PagadoEn < end
                group new { pd, pr } by new { pr.Id, pr.Nombre } into g
                orderby g.Sum(x => x.pd.MontoAsignado) descending
                select new
                {
                    ProductoId = g.Key.Id,
                    Nombre = g.Key.Nombre,
                    Cantidad = g.Sum(x => x.pd.CantidadPagada),
                    Monto = g.Sum(x => x.pd.MontoAsignado)
                }
            ).Take(10).ToListAsync(ct);

            var topProductos = top.Select(x => (x.ProductoId, x.Nombre, x.Cantidad, x.Monto)).ToList();

            return (ventasTotal, ventasEfectivo, ventasYape, ventasPlin, egresos, costosEstimados, cantidadPedidos, topProductos);
        }
    }
}
