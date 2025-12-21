using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface ICajaRepository
    {
        Task<CajaSesion?> GetCajaAbiertaAsync(CancellationToken ct);
        Task AddCajaSesionAsync(CajaSesion sesion, CancellationToken ct);

        Task AddPagoAsync(Pago pago, CancellationToken ct);
        Task AddMovimientoAsync(MovimientoCaja mov, CancellationToken ct);
        Task<decimal> CalcularEfectivoEsperadoAsync(Guid cajaSesionId, CancellationToken ct);

    }
}
