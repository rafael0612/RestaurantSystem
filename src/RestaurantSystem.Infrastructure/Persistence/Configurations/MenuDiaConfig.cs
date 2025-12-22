using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class MenuDiaConfig : IEntityTypeConfiguration<MenuDia>
    {
        public void Configure(EntityTypeBuilder<MenuDia> b)
        {
            b.ToTable("MenuDia");
            b.HasKey(x => x.Id);

            b.Property(x => x.Fecha)
                .HasColumnType("date")
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v))
                .IsRequired();

            b.HasIndex(x => x.Fecha).IsUnique();

            b.Property(x => x.Activo).IsRequired();

            // Relación 1..* (MenuDia -> Items)
            //b.HasMany<MenuDiaItem>("_items")
            //    .WithOne()
            //    .HasForeignKey(x => x.MenuDiaId)
            //    .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.MenuDiaId)
                .OnDelete(DeleteBehavior.Cascade);

            //b.Navigation("_items").UsePropertyAccessMode(PropertyAccessMode.Field);
            b.Navigation(x => x.Items)
                .HasField("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
