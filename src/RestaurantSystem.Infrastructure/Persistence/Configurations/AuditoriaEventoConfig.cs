using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class AuditoriaEventoConfig : IEntityTypeConfiguration<AuditoriaEvento>
    {
        public void Configure(EntityTypeBuilder<AuditoriaEvento> b)
        {
            b.ToTable("AuditoriaEvento");
            b.HasKey(x => x.Id);

            b.Property(x => x.Fecha).IsRequired();
            b.Property(x => x.UsuarioId).IsRequired();

            b.Property(x => x.Accion).HasMaxLength(80).IsRequired();
            b.Property(x => x.Entidad).HasMaxLength(80).IsRequired();
            b.Property(x => x.EntidadId).IsRequired();
            b.Property(x => x.Motivo).HasMaxLength(250);

            b.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.Entidad, x.EntidadId, x.Fecha });
            b.HasIndex(x => x.UsuarioId);
        }
    }
}
