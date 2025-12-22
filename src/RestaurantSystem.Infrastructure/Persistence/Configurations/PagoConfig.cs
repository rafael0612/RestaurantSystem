using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class PagoConfig : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> b)
        {
            b.ToTable("Pago");
            b.HasKey(x => x.Id);

            b.Property(x => x.CuentaId).IsRequired();
            b.Property(x => x.CajaSesionId).IsRequired();
            b.Property(x => x.PagadoEn).IsRequired();

            b.Property(x => x.Total).HasPrecision(18, 2);
            b.Property(x => x.Anulado).IsRequired();
            b.Property(x => x.MotivoAnulacion).HasMaxLength(250);

            b.HasOne<CajaSesion>()
                .WithMany()
                .HasForeignKey(x => x.CajaSesionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Metodos y Detalles (cascade seguro desde Pago)
            //b.HasMany<PagoMetodo>("_metodos")
            //    .WithOne()
            //    .HasForeignKey(x => x.PagoId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.HasMany<PagoDetalle>("_detalles")
            //    .WithOne()
            //    .HasForeignKey(x => x.PagoId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Metodos (navegación pública) + backing field "_metodos"
            b.HasMany(x => x.Metodos)
                .WithOne()
                .HasForeignKey(x => x.PagoId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(x => x.Metodos)
                .HasField("_metodos")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Detalles (navegación pública) + backing field "_detalles"
            b.HasMany(x => x.Detalles)
                .WithOne()
                .HasForeignKey(x => x.PagoId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(x => x.Detalles)
                .HasField("_detalles")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            //b.Navigation("_metodos").UsePropertyAccessMode(PropertyAccessMode.Field);
            //b.Navigation("_detalles").UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasIndex(x => new { x.CajaSesionId, x.PagadoEn });
            b.HasIndex(x => x.CuentaId);
        }
    }
}
