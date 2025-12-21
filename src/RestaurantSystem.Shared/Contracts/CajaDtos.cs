using RestaurantSystem.Shared.Enums;

namespace RestaurantSystem.Shared.Contracts;

public record AbrirCajaRequest(decimal MontoApertura);

public record RegistrarEgresoRequest(decimal Monto, string Motivo);

public record PagoMetodoRequest(MetodoPago Metodo, decimal Monto, string? ReferenciaOperacion);

public record PagoDetalleRequest(Guid ComandaDetalleId, int CantidadPagada);

public record RegistrarPagoRequest(
    Guid CuentaId,
    List<PagoDetalleRequest> Detalles,
    List<PagoMetodoRequest> Metodos,
    bool ImprimirTicket
);

public record PagoResultDto(Guid PagoId, decimal Total, DateTime PagadoEn);

public record CerrarCajaRequest(decimal MontoContado, string? MotivoDiferencia);