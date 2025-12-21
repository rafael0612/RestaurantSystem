using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface IProductoRepository
    {
        Task<Producto?> GetByIdAsync(Guid productoId, CancellationToken ct);
        Task<List<Producto>> GetMenuDiaProductosAsync(DateOnly fecha, CancellationToken ct);
    }
}
