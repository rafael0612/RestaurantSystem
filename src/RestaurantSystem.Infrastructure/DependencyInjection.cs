using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Infrastructure.Persistence;
using RestaurantSystem.Infrastructure.Persistence.Queries;
using RestaurantSystem.Infrastructure.Persistence.Repositories;

namespace RestaurantSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("SqlServer")
                     ?? throw new InvalidOperationException("ConnectionString 'SqlServer' no configurada.");

            services.AddDbContext<RestaurantSystemDbContext>(options =>
            {
                options.UseSqlServer(cs, sql =>
                {
                    // Migraciones en el assembly de Infrastructure
                    sql.MigrationsAssembly(typeof(RestaurantSystemDbContext).Assembly.FullName);
                    sql.EnableRetryOnFailure(5);
                });

                // Recomendado para producción (menos tracking)
                // options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);              
            });

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            services.AddScoped<IMesaRepository, MesaRepository>();
            services.AddScoped<ICuentaRepository, CuentaRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IComandaRepository, ComandaRepository>();
            services.AddScoped<ICajaRepository, CajaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Query repositories
            services.AddScoped<IKdsQueryRepository, KdsQueryRepository>();
            services.AddScoped<IReporteRepository, ReporteRepository>();

            return services;
        }
    }
}
