using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities
{
    public class AuditoriaEvento : AggregateRoot
    {
        private AuditoriaEvento() { }

        public AuditoriaEvento(Guid usuarioId, string accion, string entidad, Guid entidadId, string? motivo = null)
        {
            Guard.AgainstNullOrEmpty(accion, "Acción requerida.");
            Guard.AgainstNullOrEmpty(entidad, "Entidad requerida.");

            UsuarioId = usuarioId;
            Accion = accion.Trim();
            Entidad = entidad.Trim();
            EntidadId = entidadId;
            Motivo = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim();

            Fecha = DateTime.UtcNow;
        }

        public DateTime Fecha { get; private set; }
        public Guid UsuarioId { get; private set; }

        public string Accion { get; private set; } = default!;
        public string Entidad { get; private set; } = default!;
        public Guid EntidadId { get; private set; }
        public string? Motivo { get; private set; }
    }
}
