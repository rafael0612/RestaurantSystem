using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class PagoMetodoConfig : IEntityTypeConfiguration<PagoMetodo>
    {
        public void Configure(EntityTypeBuilder<PagoMetodo> b)
        {
            b.ToTable("PagoMetodo");
            b.HasKey(x => x.Id);

            b.Property(x => x.PagoId).IsRequired();
            b.Property(x => x.Metodo).IsRequired();
            b.Property(x => x.Monto).HasPrecision(18, 2);
            b.Property(x => x.ReferenciaOperacion).HasMaxLength(60);

            // Un metodo por pago (evita 2 filas "Yape" para el mismo pago)
            b.HasIndex(x => new { x.PagoId, x.Metodo }).IsUnique();

            b.HasIndex(x => x.PagoId);
        }
    }
}
