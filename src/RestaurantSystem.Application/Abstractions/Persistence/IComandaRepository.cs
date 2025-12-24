using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface IComandaRepository
    {
        Task<Comanda?> GetByIdAsync(Guid comandaId, bool includeDetails, CancellationToken ct);
        Task<ComandaDetalle?> GetDetalleByIdAsync(Guid detalleId, CancellationToken ct);
        Task AddAsync(Comanda comanda, CancellationToken ct);
        Task AddDetalleAsync(ComandaDetalle detalle, CancellationToken ct);

    }
}
