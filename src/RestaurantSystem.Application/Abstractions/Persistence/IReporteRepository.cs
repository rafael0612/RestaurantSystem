namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface IReporteRepository
    {
        Task<(decimal ventasTotal, decimal ventasEfectivo, decimal ventasYape, decimal ventasPlin,
          decimal egresos, decimal costosEstimados, int cantidadPedidos,
          List<(Guid productoId, string nombre, int cantidad, decimal monto)> topProductos)>
        GetReporteDiarioAsync(DateOnly fecha, CancellationToken ct);
    }
}
