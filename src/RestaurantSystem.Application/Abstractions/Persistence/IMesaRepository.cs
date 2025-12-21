using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface IMesaRepository
    {
        Task<List<Mesa>> GetAllAsync(CancellationToken ct);
        Task<Mesa?> GetByIdAsync(Guid mesaId, CancellationToken ct);
    }
}
