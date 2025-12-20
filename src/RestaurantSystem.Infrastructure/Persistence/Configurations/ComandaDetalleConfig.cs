using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class ComandaDetalleConfig : IEntityTypeConfiguration<ComandaDetalle>
    {
        public void Configure(EntityTypeBuilder<ComandaDetalle> b)
        {
            b.ToTable("ComandaDetalle");
            b.HasKey(x => x.Id);

            b.Property(x => x.ComandaId).IsRequired();
            b.Property(x => x.ProductoId).IsRequired();

            b.Property(x => x.Cantidad).IsRequired();
            b.Property(x => x.PrecioUnitario).HasPrecision(18, 2);
            b.Property(x => x.CostoUnitarioEstandar).HasPrecision(18, 2);

            b.Property(x => x.Observacion).HasMaxLength(250);
            b.Property(x => x.EstadoCocina).IsRequired();

            b.Property(x => x.CantidadPagada).IsRequired();
            b.Property(x => x.Anulado).IsRequired();

            b.HasOne<Producto>()
                .WithMany()
                .HasForeignKey(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.ComandaId, x.EstadoCocina });
            b.HasIndex(x => x.ProductoId);
        }
    }
}
