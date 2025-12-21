using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Dtos;

public record MesaResumenDto(
    Guid MesaId,
    string Nombre,
    EstadoMesa Estado,
    int? NroPersonas,
    Guid? CuentaActivaId
);

public record AbrirCuentaRequest(
    TipoCuenta Tipo,
    Guid? MesaId,
    int? NroPersonas,
    string? ObservacionGeneral
);

public record CuentaDetalleDto(
    Guid CuentaId,
    TipoCuenta Tipo,
    EstadoCuenta Estado,
    Guid? MesaId,
    string? MesaNombre,
    DateTime AperturaEn,
    decimal TotalConsumido,
    decimal TotalPagado,
    decimal SaldoPendiente
);

public record CrearComandaResponse(Guid ComandaId, int NumeroSecuencia);

public record AgregarItemRequest(Guid ProductoId, int Cantidad, string? Observacion);

public record EnviarACocinaRequest(bool ImprimirComanda);
