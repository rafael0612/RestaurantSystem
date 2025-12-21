namespace RestaurantSystem.Application.Abstractions.Security
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        string Role { get; } // "Mesero", "Caja", etc. (del JWT)
    }
}
