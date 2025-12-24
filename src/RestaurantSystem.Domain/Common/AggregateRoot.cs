namespace RestaurantSystem.Domain.Common
{
    public abstract class AggregateRoot : Entity
    {
        /// <summary>
        /// Concurrencia optimista. Se configura como ROWVERSION/TIMESTAMP en EF Core.
        /// </summary>
        public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();
    }
}
