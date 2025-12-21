using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Services.Rules
{
    public static class PagoRules
    {
        public static decimal CalcularSubtotalPorDetalles(IEnumerable<(ComandaDetalle item, int cantidad)> items)
            => items.Sum(x => x.cantidad * x.item.PrecioUnitario);
    }
}
