using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities
{
    public class Producto : AggregateRoot
    {
        private Producto() { }

        public Producto(string nombre, decimal precio, decimal costoEstandar, string tipo)
        {
            Guard.AgainstNullOrEmpty(nombre, "Nombre de producto es requerido.");
            Guard.AgainstNullOrEmpty(tipo, "Tipo de producto es requerido.");
            Guard.AgainstNegative(precio, "Precio no puede ser negativo.");
            Guard.AgainstNegative(costoEstandar, "Costo estándar no puede ser negativo.");

            Nombre = nombre.Trim();
            Precio = precio;
            CostoEstandar = costoEstandar;
            Tipo = tipo.Trim();
            Activo = true;
        }

        public string Nombre { get; private set; } = default!;
        public decimal Precio { get; private set; }
        public decimal CostoEstandar { get; private set; }
        public string Tipo { get; private set; } = "Plato";
        public bool Activo { get; private set; } = true;

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            Guard.AgainstNegative(nuevoPrecio, "Precio no puede ser negativo.");
            Precio = nuevoPrecio;
        }

        public void ActualizarCostoEstandar(decimal nuevoCosto)
        {
            Guard.AgainstNegative(nuevoCosto, "Costo estándar no puede ser negativo.");
            CostoEstandar = nuevoCosto;
        }

        public void Desactivar() => Activo = false;
        public void Activar() => Activo = true;
    }
}
