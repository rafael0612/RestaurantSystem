namespace RestaurantSystem.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Fecha de creación lógica (útil para auditoría básica).
        /// </summary>
        public DateTime CreadoEn { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// Concurrencia optimista. Se configura como ROWVERSION/TIMESTAMP en EF Core.
        /// </summary>
        public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();
    }
}
