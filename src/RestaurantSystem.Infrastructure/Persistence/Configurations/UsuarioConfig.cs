using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> b)
        {
            b.ToTable("Usuario");

            b.HasKey(x => x.Id);

            b.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            b.Property(x => x.Username).HasMaxLength(60).IsRequired();
            b.Property(x => x.PasswordHash).HasMaxLength(250).IsRequired();

            b.HasIndex(x => x.Username).IsUnique();

            b.Property(x => x.Activo).IsRequired();
            b.Property(x => x.Rol).IsRequired();

            // RowVersion se configura globalmente por convención
        }
    }
}
