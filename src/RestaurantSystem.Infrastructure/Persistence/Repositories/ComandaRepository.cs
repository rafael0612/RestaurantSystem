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
            IQueryable<Comanda> q = _db.Comandas;

            if (includeDetails)
                q = q.Include("_detalles");

            return await q.FirstOrDefaultAsync(c => c.Id == comandaId, ct);
        }

        public Task<ComandaDetalle?> GetDetalleByIdAsync(Guid detalleId, CancellationToken ct)
            => _db.ComandaDetalles.FirstOrDefaultAsync(d => d.Id == detalleId, ct);

        public Task AddAsync(Comanda comanda, CancellationToken ct)
        {
            _db.Comandas.Add(comanda);
            return Task.CompletedTask;
        }
    }
}
