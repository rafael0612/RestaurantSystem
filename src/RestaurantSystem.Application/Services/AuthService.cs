using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Abstractions.Security;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct);
    }

    public sealed class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenGenerator _jwt;

        public AuthService(IUsuarioRepository users, IPasswordHasher hasher, IJwtTokenGenerator jwt)
        {
            _users = users;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct)
        {
            var username = (req.Username ?? "").Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(req.Password))
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            var user = await _users.GetByUsernameAsync(username, ct);
            if (user is null || !user.Activo)
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            if (!_hasher.Verify(req.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            var (token, exp) = _jwt.CreateToken(user);

            return new LoginResponse(
                user.Id,
                user.Username,
                user.Rol.ToString(),   // "Mesero" / "Cocinero" / "Caja" / "Admin"
                token,
                exp
            );
        }
    }
}
