using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace RestaurantSystem.Infrastructure.Persistence.Conventions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyGlobalConventions(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // 1) Decimales por defecto (si no están configurados explícitamente)
                foreach (var property in entityType.GetProperties().Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                {
                    if (property.GetPrecision() is null && property.GetScale() is null)
                    {
                        property.SetPrecision(18);
                        property.SetScale(2);
                    }
                }

                // 2) RowVersion si existe propiedad "RowVersion"
                var rowVersionProp = entityType.ClrType.GetProperty("RowVersion", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (rowVersionProp is not null && rowVersionProp.PropertyType == typeof(byte[]))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("RowVersion")
                        .IsRowVersion()
                        .IsConcurrencyToken();
                }
            }
        }
    }
}
