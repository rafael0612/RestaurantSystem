using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly RestaurantSystemDbContext _db;
        public UsuarioRepository(RestaurantSystemDbContext db) => _db = db;

        public Task AddAsync(Usuario user, CancellationToken ct)
        {
            _db.Usuarios.Add(user);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct)
            => _db.Usuarios.AsNoTracking()
                            .AnyAsync(x => x.Username == username, ct);

        public Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct)
            => _db.Usuarios.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<Usuario?> GetByUsernameAsync(string username, CancellationToken ct)
            => _db.Usuarios.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Username == username, ct);

        public Task<List<Usuario>> ListAsync(CancellationToken ct)
            => _db.Usuarios.AsNoTracking()
                            .OrderBy(x => x.Username)
                            .ToListAsync(ct);
    }
}
