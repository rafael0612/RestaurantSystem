using RestaurantSystem.Application.Abstractions.Persistence;

namespace RestaurantSystem.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantSystemDbContext _db;
        public UnitOfWork(RestaurantSystemDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct)
            => _db.SaveChangesAsync(ct);
    }
}
