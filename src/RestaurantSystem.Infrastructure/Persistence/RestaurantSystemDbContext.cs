using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Infrastructure.Persistence.Conventions;


namespace RestaurantSystem.Infrastructure.Persistence
{
    public class RestaurantSystemDbContext : DbContext
    {
        public RestaurantSystemDbContext(DbContextOptions<RestaurantSystemDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Producto> Productos => Set<Producto>();

        public DbSet<MenuDia> MenusDia => Set<MenuDia>();
        public DbSet<MenuDiaItem> MenuDiaItems => Set<MenuDiaItem>();

        public DbSet<Mesa> Mesas => Set<Mesa>();
        public DbSet<Cuenta> Cuentas => Set<Cuenta>();

        public DbSet<Comanda> Comandas => Set<Comanda>();
        public DbSet<ComandaDetalle> ComandaDetalles => Set<ComandaDetalle>();

        public DbSet<CajaSesion> CajasSesion => Set<CajaSesion>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<PagoMetodo> PagoMetodos => Set<PagoMetodo>();
        public DbSet<PagoDetalle> PagoDetalles => Set<PagoDetalle>();

        public DbSet<MovimientoCaja> MovimientosCaja => Set<MovimientoCaja>();
        public DbSet<AuditoriaEvento> AuditoriaEventos => Set<AuditoriaEvento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantSystemDbContext).Assembly);

            // Convenciones globales (decimales, rowversion, etc.)
            modelBuilder.ApplyGlobalConventions();

            base.OnModelCreating(modelBuilder);
        }
    }
}
