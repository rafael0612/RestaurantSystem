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
            b.Property(x => x.PrecioUnitario).IsRequired();
            b.Property(x => x.CostoUnitarioEstandar).IsRequired();

            b.Property(x => x.Observacion).HasMaxLength(250);
            b.Property(x => x.EstadoCocina).IsRequired();

            b.Property(x => x.CantidadPagada).IsRequired();
            b.Property(x => x.Anulado).IsRequired();

            // Ignorar calculadas
            b.Ignore(x => x.TotalLinea);
            b.Ignore(x => x.CostoLinea);
            b.Ignore(x => x.CantidadPendientePago);

            b.HasOne<Producto>()
                .WithMany()
                .HasForeignKey(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices útiles
            b.HasIndex(x => x.ComandaId);
            b.HasIndex(x => new { x.ComandaId, x.Anulado });
            b.HasIndex(x => new { x.ComandaId, x.EstadoCocina });
            b.HasIndex(x => x.ProductoId);
        }
    }
}
