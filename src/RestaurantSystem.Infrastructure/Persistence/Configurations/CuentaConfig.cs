using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class CuentaConfig : IEntityTypeConfiguration<Cuenta>
    {
        public void Configure(EntityTypeBuilder<Cuenta> b)
        {
            b.ToTable("Cuenta");
            b.HasKey(x => x.Id);

            b.Property(x => x.Tipo).IsRequired();
            b.Property(x => x.Estado).IsRequired();

            b.Property(x => x.ObservacionGeneral).HasMaxLength(300);

            b.Property(x => x.AperturaEn).IsRequired();
            b.Property(x => x.CierreEn);

            // Mesa opcional (para llevar => MesaId null)
            b.HasOne<Mesa>()
                .WithMany()
                .HasForeignKey(x => x.MesaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario creador
            b.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.CreadaPorUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Colecciones privadas (backing fields)
            b.HasMany<Comanda>("_comandas")
                .WithOne()
                .HasForeignKey(x => x.CuentaId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany<Pago>("_pagos")
                .WithOne()
                .HasForeignKey(x => x.CuentaId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Navigation("_comandas").UsePropertyAccessMode(PropertyAccessMode.Field);
            b.Navigation("_pagos").UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasIndex(x => new { x.MesaId, x.Estado });
            b.HasIndex(x => x.AperturaEn);
        }
    }
}
