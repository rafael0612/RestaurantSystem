using D = RestaurantSystem.Domain.Enums;
using S = RestaurantSystem.Shared.Enums;

namespace RestaurantSystem.Application.Common
{
    public static class EnumMapper
    {
        // Shared -> Domain
        public static D.TipoCuenta ToDomain(this S.TipoCuenta v) => (D.TipoCuenta)(int)v;
        public static D.EstadoCuenta ToDomain(this S.EstadoCuenta v) => (D.EstadoCuenta)(int)v;
        public static D.EstadoMesa ToDomain(this S.EstadoMesa v) => (D.EstadoMesa)(int)v;
        public static D.EstadoComanda ToDomain(this S.EstadoComanda v) => (D.EstadoComanda)(int)v;
        public static D.EstadoCocinaItem ToDomain(this S.EstadoCocinaItem v) => (D.EstadoCocinaItem)(int)v;
        public static D.MetodoPago ToDomain(this S.MetodoPago v) => (D.MetodoPago)(int)v;

        public static D.EstadoCocinaItem? ToDomain(this S.EstadoCocinaItem? v)
            => v.HasValue ? (D.EstadoCocinaItem?)(D.EstadoCocinaItem)(int)v.Value : null;

        // Domain -> Shared
        public static S.TipoCuenta ToShared(this D.TipoCuenta v) => (S.TipoCuenta)(int)v;
        public static S.EstadoCuenta ToShared(this D.EstadoCuenta v) => (S.EstadoCuenta)(int)v;
        public static S.EstadoMesa ToShared(this D.EstadoMesa v) => (S.EstadoMesa)(int)v;
        public static S.EstadoComanda ToShared(this D.EstadoComanda v) => (S.EstadoComanda)(int)v;
        public static S.EstadoCocinaItem ToShared(this D.EstadoCocinaItem v) => (S.EstadoCocinaItem)(int)v;
        public static S.MetodoPago ToShared(this D.MetodoPago v) => (S.MetodoPago)(int)v;
    }
}
