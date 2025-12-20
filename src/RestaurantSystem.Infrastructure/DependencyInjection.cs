using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Infrastructure.Persistence;

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

            return services;
        }
    }
}
