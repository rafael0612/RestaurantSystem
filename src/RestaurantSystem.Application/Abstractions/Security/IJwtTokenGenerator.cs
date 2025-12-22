using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Abstractions.Security
{
    public interface IJwtTokenGenerator
    {
        (string token, DateTime expiresAtUtc) CreateToken(Usuario user);
    }
}
