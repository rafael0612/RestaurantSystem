using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly RestaurantSystemDbContext _db;
        public ProductoRepository(RestaurantSystemDbContext db) => _db = db;

        public Task<Producto?> GetByIdAsync(Guid productoId, CancellationToken ct)
            => _db.Productos.FirstOrDefaultAsync(p => p.Id == productoId, ct);

        public async Task<List<Producto>> GetMenuDiaProductosAsync(DateOnly fecha, CancellationToken ct)
        {
            // 1) buscar MenuDia por fecha
            var menuId = await _db.MenusDia
                .Where(m => m.Activo && m.Fecha == fecha)
                .Select(m => (Guid?)m.Id)
                .FirstOrDefaultAsync(ct);

            if (!menuId.HasValue) return [];

            // 2) traer productos del menú
            var productos = await _db.MenuDiaItems
                .Where(i => i.MenuDiaId == menuId.Value)
                .Join(_db.Productos,
                    i => i.ProductoId,
                    p => p.Id,
                    (i, p) => p)
                .Where(p => p.Activo)
                .OrderBy(p => p.Tipo).ThenBy(p => p.Nombre)
                .ToListAsync(ct);

            return productos;
        }
    }
}
