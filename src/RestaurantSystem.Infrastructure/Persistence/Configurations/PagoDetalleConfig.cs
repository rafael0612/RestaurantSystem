using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class PagoDetalleConfig : IEntityTypeConfiguration<PagoDetalle>
    {
        public void Configure(EntityTypeBuilder<PagoDetalle> b)
        {
            b.ToTable("PagoDetalle");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).ValueGeneratedNever();

            b.Property(x => x.PagoId).IsRequired();
            b.Property(x => x.ComandaDetalleId).IsRequired();

            b.Property(x => x.CantidadPagada).IsRequired();
            b.Property(x => x.MontoAsignado).HasPrecision(18, 2);

            b.HasOne<ComandaDetalle>()
                .WithMany()
                .HasForeignKey(x => x.ComandaDetalleId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.ComandaDetalleId);
            b.HasIndex(x => x.PagoId);
        }
    }
}
