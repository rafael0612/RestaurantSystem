using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class MovimientoCajaConfig : IEntityTypeConfiguration<MovimientoCaja>
    {
        public void Configure(EntityTypeBuilder<MovimientoCaja> b)
        {
            b.ToTable("MovimientoCaja");
            b.HasKey(x => x.Id);

            b.Property(x => x.CajaSesionId).IsRequired();
            b.Property(x => x.UsuarioId).IsRequired();
            b.Property(x => x.Tipo).IsRequired();

            b.Property(x => x.Monto).HasPrecision(18, 2);
            b.Property(x => x.Motivo).HasMaxLength(250).IsRequired();
            b.Property(x => x.Fecha).IsRequired();

            b.HasOne<CajaSesion>()
                .WithMany()
                .HasForeignKey(x => x.CajaSesionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.CajaSesionId, x.Fecha });
        }
    }
}
