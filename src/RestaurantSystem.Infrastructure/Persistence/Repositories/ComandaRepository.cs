using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Repositories
{
    public class ComandaRepository : IComandaRepository
    {
        private readonly RestaurantSystemDbContext _db;
        public ComandaRepository(RestaurantSystemDbContext db) => _db = db;

        public async Task<Comanda?> GetByIdAsync(Guid comandaId, bool includeDetails, CancellationToken ct)
        {
            IQueryable<Comanda> q = _db.Comandas.AsTracking();

            if (includeDetails)
                q = q.Include(c => c.Detalles);
            //q = q.Include("_detalles");

            return await q.FirstOrDefaultAsync(c => c.Id == comandaId, ct);
        }

        public Task<ComandaDetalle?> GetDetalleByIdAsync(Guid detalleId, CancellationToken ct)
            => _db.ComandaDetalles.FirstOrDefaultAsync(d => d.Id == detalleId, ct);

        public Task AddAsync(Comanda comanda, CancellationToken ct)
        {
            _db.Comandas.Add(comanda);
            return Task.CompletedTask;
        }
        public Task AddDetalleAsync(ComandaDetalle detalle, CancellationToken ct)
        {
            var entry = _db.Entry(detalle);

            if (entry.State == EntityState.Detached)
                _db.ComandaDetalles.Add(detalle);
            else
                entry.State = EntityState.Added;

            return Task.CompletedTask;
        }
    }
}
