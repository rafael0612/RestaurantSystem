using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class MesaConfig : IEntityTypeConfiguration<Mesa>
    {
        public void Configure(EntityTypeBuilder<Mesa> b)
        {
            b.ToTable("Mesa");
            b.HasKey(x => x.Id);

            b.Property(x => x.Nombre).HasMaxLength(30).IsRequired();
            b.HasIndex(x => x.Nombre).IsUnique();

            b.Property(x => x.Estado).IsRequired();
            b.Property(x => x.NroPersonas);

            b.HasIndex(x => x.Estado);
        }
    }
}
