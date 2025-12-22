using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Abstractions.Security;
using RestaurantSystem.Application.Common;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.Application.Services
{
    public interface IUsuarioService
    {
        Task<Guid> CrearAsync(CrearUsuarioRequest req, CancellationToken ct);
        Task<UsuarioDto> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<UsuarioDto>> ListAsync(CancellationToken ct);

        Task CambiarRolAsync(Guid id, CambiarRolRequest req, CancellationToken ct);
        Task CambiarEstadoAsync(Guid id, CambiarEstadoRequest req, CancellationToken ct);
        Task ResetPasswordAsync(Guid id, ResetPasswordRequest req, CancellationToken ct);
    }

    public sealed class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _uow;

        public UsuarioService(IUsuarioRepository users, IPasswordHasher hasher, IUnitOfWork uow)
        {
            _users = users;
            _hasher = hasher;
            _uow = uow;
        }

        public async Task<Guid> CrearAsync(CrearUsuarioRequest req, CancellationToken ct)
        {
            var username = (req.Username ?? "").Trim();
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Username requerido.");

            if (await _users.ExistsByUsernameAsync(username, ct))
                throw new InvalidOperationException("Username ya existe.");

            if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
                throw new InvalidOperationException("Password inválido (mínimo 6).");

            var hash = _hasher.Hash(req.Password);

            var user = new Usuario(
                username: username,
                passwordHash: hash,
                nombre: req.Nombre,
                apellido: req.Apellido,
                rol: req.Rol.ToDomain()
            );

            await _users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            return user.Id;
        }

        public async Task<UsuarioDto> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var u = await _users.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Usuario no existe.");
            return ToDto(u);
        }

        public async Task<List<UsuarioDto>> ListAsync(CancellationToken ct)
        {
            var list = await _users.ListAsync(ct);
            return list.Select(ToDto).ToList();
        }

        public async Task CambiarRolAsync(Guid id, CambiarRolRequest req, CancellationToken ct)
        {
            var u = await _users.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Usuario no existe.");
            u.CambiarRol(req.Rol.ToDomain());

            await _uow.SaveChangesAsync(ct);
        }

        public async Task CambiarEstadoAsync(Guid id, CambiarEstadoRequest req, CancellationToken ct)
        {
            var u = await _users.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Usuario no existe.");

            if (req.Activo) u.Activar();
            else u.Desactivar();

            await _uow.SaveChangesAsync(ct);
        }

        public async Task ResetPasswordAsync(Guid id, ResetPasswordRequest req, CancellationToken ct)
        {
            var u = await _users.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Usuario no existe.");

            if (string.IsNullOrWhiteSpace(req.NewPassword) || req.NewPassword.Length < 6)
                throw new InvalidOperationException("Password inválido (mínimo 6).");

            var hash = _hasher.Hash(req.NewPassword);
            u.CambiarPasswordHash(hash);

            await _uow.SaveChangesAsync(ct);
        }

        private static UsuarioDto ToDto(Usuario u)
            => new UsuarioDto(
                u.Id,
                u.Username,
                u.Nombre,
                u.Apellido,
                u.NombreCompleto,
                u.Rol.ToShared(),
                u.Activo
            );
    }
}
