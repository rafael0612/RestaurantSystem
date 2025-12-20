using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class ComandaConfig : IEntityTypeConfiguration<Comanda>
    {
        public void Configure(EntityTypeBuilder<Comanda> b)
        {
            b.ToTable("Comanda");
            b.HasKey(x => x.Id);

            b.Property(x => x.CuentaId).IsRequired();
            b.Property(x => x.Estado).IsRequired();
            b.Property(x => x.CreadaEn).IsRequired();
            b.Property(x => x.CreadaPorUsuarioId).IsRequired();
            b.Property(x => x.NumeroSecuencia).IsRequired();

            // Unicidad por ronda dentro de una cuenta
            b.HasIndex(x => new { x.CuentaId, x.NumeroSecuencia }).IsUnique();

            b.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.CreadaPorUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Detalles (cascade seguro aquí)
            b.HasMany<ComandaDetalle>("_detalles")
                .WithOne()
                .HasForeignKey(x => x.ComandaId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation("_detalles").UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasIndex(x => new { x.CuentaId, x.CreadaEn });
        }
    }
}
