using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class MenuDiaItemConfig : IEntityTypeConfiguration<MenuDiaItem>
    {
        public void Configure(EntityTypeBuilder<MenuDiaItem> b)
        {
            b.ToTable("MenuDiaItem");
            b.HasKey(x => x.Id);

            b.Property(x => x.MenuDiaId).IsRequired();
            b.Property(x => x.ProductoId).IsRequired();

            b.HasIndex(x => new { x.MenuDiaId, x.ProductoId }).IsUnique();

            b.HasOne<Producto>()
                .WithMany()
                .HasForeignKey(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
