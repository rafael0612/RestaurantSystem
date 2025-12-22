namespace RestaurantSystem.Shared.Contracts;

public record ProductoDto(
    Guid Id,
    string Nombre,
    decimal Precio,
    decimal CostoEstandar,
    string Tipo,
    bool Activo
);