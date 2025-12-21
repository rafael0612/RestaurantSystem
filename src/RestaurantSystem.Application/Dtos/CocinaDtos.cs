using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Dtos;

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
    string Origen, // "Mesa 1" o "Para llevar"
    DateTime CreadaEn,
    EstadoComanda Estado,
    List<KdsItemDto> Items
);

public record CambiarEstadoCocinaItemRequest(Guid ComandaDetalleId, EstadoCocinaItem NuevoEstado);
