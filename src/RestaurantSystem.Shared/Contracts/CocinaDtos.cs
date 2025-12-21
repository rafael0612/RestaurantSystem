using RestaurantSystem.Shared.Enums;

namespace RestaurantSystem.Shared.Contracts;

public record KdsItemDto(
    Guid ComandaDetalleId,
    string Producto,
    int Cantidad,
    string? Observacion,
    EstadoCocinaItem Estado
);

public record KdsCardDto(
    Guid ComandaId,
    Guid CuentaId,
    string Origen,
    DateTime CreadaEn,
    EstadoComanda Estado,
    List<KdsItemDto> Items
);