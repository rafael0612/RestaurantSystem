using RestaurantSystem.Infrastructure.Persistence;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Application.Abstractions.Security;
using Microsoft.EntityFrameworkCore;

namespace RestaurantSystem.API.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
        {
            var db = sp.GetRequiredService<RestaurantSystemDbContext>();
            var hasher = sp.GetRequiredService<IPasswordHasher>();

            // Si quieres auto-aplicar migraciones al levantar la API:
            await db.Database.MigrateAsync(ct);

            await SeedAdminAsync(db, hasher, ct);
            await SeedMesasAsync(db, ct);
            await SeedProductosAsync(db, ct);
            await SeedMenuDiaAsync(db, ct);            
        }
        private static async Task SeedAdminAsync(RestaurantSystemDbContext db, IPasswordHasher hasher, CancellationToken ct)
        {
            // Admin por defecto (solo si no existe)
            const string adminUsername = "admin";

            var exists = await db.Usuarios.AnyAsync(u => u.Username == adminUsername, ct);
            if (exists) return;

            var hash = hasher.Hash("Admin123*"); // Cambia luego en producción

            var admin = new Usuario(
                username: adminUsername,
                passwordHash: hash,
                nombre: "Admin",
                apellido: "Sistema",
                rol: RolUsuario.Admin
            );

            db.Usuarios.Add(admin);
            await db.SaveChangesAsync(ct);
        }
        private static async Task SeedMesasAsync(RestaurantSystemDbContext db, CancellationToken ct)
        {
            if (await db.Mesas.AnyAsync(ct)) return;

            // Firma real: Mesa(string nombre) -> Estado Libre por defecto
            db.Mesas.AddRange(
                new Mesa("Mesa 1"),
                new Mesa("Mesa 2"),
                new Mesa("Mesa 3"),
                new Mesa("Mesa 4")
            );
            await db.SaveChangesAsync(ct);
        }
        private static async Task SeedProductosAsync(RestaurantSystemDbContext db, CancellationToken ct)
        {
            // Seed idempotente por nombre (no solo "si hay alguno")
            var existentes = await db.Productos.AsNoTracking().Select(p => p.Nombre).ToListAsync(ct);
            var set = new HashSet<string>(existentes, StringComparer.OrdinalIgnoreCase);

            // Firma real: Producto(string nombre, decimal precio, decimal costoEstandar, string tipo)
            var nuevos = new List<Producto>();

            void AddIfMissing(string nombre, decimal precio, decimal costo, string tipo)
            {
                if (set.Contains(nombre)) return;
                nuevos.Add(new Producto(nombre, precio, costo, tipo));
                set.Add(nombre);
            }

            // Platos (Menú del día)
            AddIfMissing("Menú - Pollo a la brasa", 12.00m, 6.00m, "Menu");
            AddIfMissing("Menú - Lomo saltado", 12.00m, 6.50m, "Menu");
            AddIfMissing("Menú - Tallarín saltado", 11.00m, 5.80m, "Menu");

            // Bebidas
            AddIfMissing("Inca Kola 500ml", 4.00m, 2.20m, "Bebida");
            AddIfMissing("Coca Cola 500ml", 4.00m, 2.20m, "Bebida");
            AddIfMissing("Agua 500ml", 3.00m, 1.20m, "Bebida");

            // Postre
            AddIfMissing("Postre del día", 5.00m, 2.50m, "Postre");

            // Combo (opcional, pero te sirve por tu estrategia de precios)
            AddIfMissing("Combo Menú + Bebida", 15.00m, 8.50m, "Combo");

            if (nuevos.Count == 0) return;

            db.Productos.AddRange(nuevos);
            await db.SaveChangesAsync(ct);
        }

        private static async Task SeedMenuDiaAsync(RestaurantSystemDbContext db, CancellationToken ct)
        {
            // Para DEV en tu PC: DateOnly con hora local
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            // Traemos o creamos menú del día
            var menu = await db.MenusDia
                .Include(m => m.Items)
                .FirstOrDefaultAsync(m => m.Fecha == hoy, ct);

            // Productos tipo "Menu" activos
            var productosMenu = await db.Productos
                .AsNoTracking()
                .Where(p => p.Activo && p.Tipo == "Menu")
                .Select(p => p.Id)
                .ToListAsync(ct);

            if (productosMenu.Count == 0) return;

            if (menu is null)
            {
                // Firma real: MenuDia(DateOnly fecha)
                menu = new MenuDia(hoy);

                // Agrega items vía método de dominio (usa _items)
                foreach (var pid in productosMenu)
                    menu.AgregarProducto(pid);

                db.MenusDia.Add(menu);
                await db.SaveChangesAsync(ct);
                return;
            }

            // Si ya existe el menú para hoy, aseguramos que tenga los productos (idempotente)
            var actuales = menu.Items.Select(i => i.ProductoId).ToHashSet();
            var faltantes = productosMenu.Where(pid => !actuales.Contains(pid)).ToList();

            if (faltantes.Count == 0) return;

            foreach (var pid in faltantes)
                menu.AgregarProducto(pid);

            await db.SaveChangesAsync(ct);
        }
    }
}
