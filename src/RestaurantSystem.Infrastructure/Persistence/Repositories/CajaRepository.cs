using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

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
            var ingresosEfectivo = await _db.PagoMetodos
                .Where(pm => pm.Metodo == MetodoPago.Efectivo
                             && _db.Pagos.Any(p => p.Id == pm.PagoId && !p.Anulado && p.CajaSesionId == cajaSesionId))
                .SumAsync(pm => (decimal?)pm.Monto, ct) ?? 0m;

            // Egresos = suma MovimientoCaja(Egreso) de esa sesión
            var egresos = await _db.MovimientosCaja
                .Where(m => m.CajaSesionId == cajaSesionId && m.Tipo == TipoMovimientoCaja.Egreso)
                .SumAsync(m => (decimal?)m.Monto, ct) ?? 0m;

            var apertura = await _db.CajasSesion
                .Where(c => c.Id == cajaSesionId)
                .Select(c => c.MontoApertura)
                .FirstAsync(ct);

            return apertura + ingresosEfectivo - egresos;
        }
    }
}
