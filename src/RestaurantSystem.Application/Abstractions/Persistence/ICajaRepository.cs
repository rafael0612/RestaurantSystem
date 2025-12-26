using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface ICajaRepository
    {
        Task<CajaSesion?> GetCajaAbiertaAsync(CancellationToken ct);
        Task<CajaSesion?> GetSesionAbiertaPorUsuarioAsync(Guid UserId ,CancellationToken ct);
        Task<CajaSesionDto?> GetSesionAbiertaDtoAsync(CancellationToken ct);
        Task AddCajaSesionAsync(CajaSesion sesion, CancellationToken ct);

        Task AddPagoAsync(Pago pago, CancellationToken ct);
        Task AddMovimientoAsync(MovimientoCaja mov, CancellationToken ct);

        Task<decimal> CalcularEfectivoEsperadoAsync(Guid cajaSesionId, CancellationToken ct);

        Task<List<CuentaPorCobrarDto>> ListarCuentasPorCobrarAsync(CancellationToken ct);
        Task<CuentaCobroDto> GetCuentaCobroAsync(Guid cuentaId, CancellationToken ct);

        Task<ReporteDiarioCajaDto> GetReporteDiarioCajaAsync(DateOnly fecha, CancellationToken ct);

    }
}
