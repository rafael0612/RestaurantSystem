using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUsernameAsync(string username, CancellationToken ct);
        Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct);
        Task<List<Usuario>> ListAsync(CancellationToken ct);
        Task AddAsync(Usuario user, CancellationToken ct);

    }
}
