using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface ICuentaRepository
    {
        Task<Cuenta?> GetByIdAsync(Guid cuentaId, bool includeDetails, CancellationToken ct);
        Task<Cuenta?> GetCuentaActivaPorMesaAsync(Guid mesaId, CancellationToken ct);
        Task AddAsync(Cuenta cuenta, CancellationToken ct);
    }
}
