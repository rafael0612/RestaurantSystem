using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class CajaSesionConfig : IEntityTypeConfiguration<CajaSesion>
    {
        public void Configure(EntityTypeBuilder<CajaSesion> b)
        {
            b.ToTable("CajaSesion");
            b.HasKey(x => x.Id);

            b.Property(x => x.UsuarioId).IsRequired();
            b.Property(x => x.Estado).IsRequired();

            b.Property(x => x.AperturaEn).IsRequired();
            b.Property(x => x.MontoApertura).HasPrecision(18, 2);

            b.Property(x => x.CierreEn);
            b.Property(x => x.MontoCierreContado).HasPrecision(18, 2);
            b.Property(x => x.EfectivoEsperado).HasPrecision(18, 2);
            b.Property(x => x.Diferencia).HasPrecision(18, 2);

            b.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.UsuarioId, x.AperturaEn });
            b.HasIndex(x => x.Estado);
        }
    }
}
