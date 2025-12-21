using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Repositories
{
    public class MesaRepository : IMesaRepository
    {
        private readonly RestaurantSystemDbContext _db;
        public MesaRepository(RestaurantSystemDbContext db) => _db = db;

        public Task<List<Mesa>> GetAllAsync(CancellationToken ct)
            => _db.Mesas.OrderBy(m => m.Nombre).ToListAsync(ct);

        public Task<Mesa?> GetByIdAsync(Guid mesaId, CancellationToken ct)
            => _db.Mesas.FirstOrDefaultAsync(m => m.Id == mesaId, ct);
    }
}
