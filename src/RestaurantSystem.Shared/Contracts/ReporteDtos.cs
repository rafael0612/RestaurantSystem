namespace RestaurantSystem.Shared.Contracts;

public record ProductoVendidoDto(Guid ProductoId, string Nombre, int Cantidad, decimal Monto);

public record ReporteDiarioDto(
    DateOnly Fecha,
    decimal VentasTotal,
    decimal VentasEfectivo,
    decimal VentasYape,
    decimal VentasPlin,
    decimal Egresos,
    decimal CostosEstimados,
    decimal GananciaEstimada,
    int CantidadPedidos,
    List<ProductoVendidoDto> TopProductos
);