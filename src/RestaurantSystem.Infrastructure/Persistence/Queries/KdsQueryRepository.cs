using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Dtos;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Queries
{
    public class KdsQueryRepository : IKdsQueryRepository
    {
        private readonly RestaurantSystemDbContext _db;
        public KdsQueryRepository(RestaurantSystemDbContext db) => _db = db;

        public async Task<List<KdsCardDto>> GetKdsAsync(EstadoCocinaItem? estadoFiltro, CancellationToken ct)
        {
            // Nota: Cuenta no tiene navegación a Mesa; usamos join por MesaId
            // También ComandaDetalle no tiene nav a Producto; usamos join por ProductoId

            var comandasBase = _db.Comandas
                .AsNoTracking()
                .Where(c => c.Estado == EstadoComanda.EnCocina || c.Estado == EstadoComanda.Listo);

            if (estadoFiltro.HasValue)
            {
                comandasBase = comandasBase.Where(c =>
                    _db.ComandaDetalles.Any(d => d.ComandaId == c.Id && !d.Anulado && d.EstadoCocina == estadoFiltro.Value));
            }

            var cards = await (
                from c in comandasBase
                join cu in _db.Cuentas.AsNoTracking() on c.CuentaId equals cu.Id
                join m in _db.Mesas.AsNoTracking() on cu.MesaId equals m.Id into mesaJoin
                from mesa in mesaJoin.DefaultIfEmpty()
                orderby c.CreadaEn
                select new { c, cu, mesa }
            ).ToListAsync(ct);

            var comandaIds = cards.Select(x => x.c.Id).ToList();

            var items = await (
                from d in _db.ComandaDetalles.AsNoTracking()
                join p in _db.Productos.AsNoTracking() on d.ProductoId equals p.Id
                where comandaIds.Contains(d.ComandaId) && !d.Anulado
                select new
                {
                    d.ComandaId,
                    Item = new KdsItemDto(d.Id, p.Nombre, d.Cantidad, d.Observacion, d.EstadoCocina)
                }
            ).ToListAsync(ct);

            var itemsByComanda = items
                .GroupBy(x => x.ComandaId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Item).ToList());

            var result = cards.Select(x =>
            {
                var origen = x.cu.Tipo == TipoCuenta.Llevar
                    ? "Para llevar"
                    : (x.mesa?.Nombre ?? "Mesa");

                return new KdsCardDto(
                    x.c.Id,
                    x.cu.Id,
                    origen,
                    x.c.CreadaEn,
                    x.c.Estado,
                    itemsByComanda.TryGetValue(x.c.Id, out var list) ? list : new List<KdsItemDto>()
                );
            }).ToList();

            return result;
        }
    }
}
