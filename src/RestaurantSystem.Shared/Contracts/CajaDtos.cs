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
public record CerrarCajaResponse(Guid CajaSesionId, decimal EfectivoEsperado, decimal MontoContado, decimal Diferencia);

// Sesión actual de caja (para saber si está abierto)
public record CajaSesionDto(
    Guid CajaSesionId,
    DateTime AperturaEn,
    decimal MontoApertura,
    bool Abierta,
    DateTime? CierreEn,
    decimal? MontoCierreContado,
    decimal? EfectivoEsperado,
    decimal? Diferencia
);

// Cuentas que están listas para cobrar (Por Cobrar)
public record CuentaPorCobrarDto(
    Guid CuentaId,
    string Origen, // "Mesa 1" o "Para llevar"
    DateTime AperturaEn,
    decimal TotalConsumido,
    decimal TotalPagado,
    decimal SaldoPendiente
);

// Ítems pendientes de pago (por comanda detalle)
public record CobroItemDto(
    Guid ComandaDetalleId,
    int ComandaNumero,
    string Producto,
    int CantidadPendiente,
    decimal PrecioUnitario,
    string? Observacion,
    EstadoCocinaItem EstadoCocina
);

// Detalle completo para pantalla de cobro
public record CuentaCobroDto(
    Guid CuentaId,
    string Origen,
    DateTime AperturaEn,
    decimal TotalConsumido,
    decimal TotalPagado,
    decimal SaldoPendiente,
    List<CobroItemDto> Items
);

// Respuesta del pago (más útil para UI que solo PagoResultDto)
public record RegistrarPagoResponse(
    Guid PagoId,
    decimal TotalPagado,
    decimal SaldoPendiente
);

// Reporte diario CAJA (POS)
public record ReporteMetodoCajaDto(MetodoPago Metodo, decimal Total);

public record PagoResumenCajaDto(Guid PagoId, DateTime PagadoEn, string Origen, decimal Total);

public record ReporteDiarioCajaDto(
    DateOnly Fecha,
    decimal TotalVentas,
    List<ReporteMetodoCajaDto> TotalesPorMetodo,
    List<PagoResumenCajaDto> Pagos
);