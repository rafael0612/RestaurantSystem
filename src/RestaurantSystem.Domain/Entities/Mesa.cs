using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Mesa : AggregateRoot
    {
        private Mesa() { }

        public Mesa(string nombre)
        {
            Guard.AgainstNullOrEmpty(nombre, "Nombre de mesa es requerido.");
            Nombre = nombre.Trim();
            Estado = EstadoMesa.Libre;
        }

        public string Nombre { get; private set; } = default!;
        public EstadoMesa Estado { get; private set; }
        public int? NroPersonas { get; private set; }

        public void CambiarPersonas(int? nroPersonas)
        {
            if (nroPersonas.HasValue && nroPersonas.Value <= 0)
                throw new DomainException("NroPersonas debe ser mayor a 0.");
            NroPersonas = nroPersonas;
        }

        public void MarcarLibre()
        {
            Estado = EstadoMesa.Libre;
            NroPersonas = null;
        }

        public void MarcarOcupada() => Estado = EstadoMesa.Ocupada;
        public void MarcarPorCobrar() => Estado = EstadoMesa.PorCobrar;
    }
}
