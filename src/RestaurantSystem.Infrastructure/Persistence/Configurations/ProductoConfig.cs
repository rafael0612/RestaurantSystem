using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class ProductoConfig : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> b)
        {
            b.ToTable("Producto");
            b.HasKey(x => x.Id);

            b.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
            b.Property(x => x.Tipo).HasMaxLength(30).IsRequired();

            b.Property(x => x.Precio).HasPrecision(18, 2);
            b.Property(x => x.CostoEstandar).HasPrecision(18, 2);

            b.Property(x => x.Activo).IsRequired();

            b.HasIndex(x => x.Nombre);
            b.HasIndex(x => new { x.Tipo, x.Activo });
        }
    }
}