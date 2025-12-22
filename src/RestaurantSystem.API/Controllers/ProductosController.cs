using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Infrastructure.Persistence;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.API.Controllers
{
    [ApiController]
    [Route("api/productos")]
    [Authorize] // Mesero/Caja/Cocinero/Admin
    public class ProductosController : ControllerBase
    {
        private readonly RestaurantSystemDbContext _db;
        public ProductosController(RestaurantSystemDbContext db) => _db = db;

        [HttpGet("activos")]
        public async Task<ActionResult<List<ProductoDto>>> GetActivos(CancellationToken ct)
        {
            var list = await _db.Productos.AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.Tipo)
                .ThenBy(p => p.Nombre)
                .Select(p => new ProductoDto(p.Id, p.Nombre, p.Precio, p.CostoEstandar, p.Tipo, p.Activo))
                .ToListAsync(ct);

            return Ok(list);
        }
    }
}
