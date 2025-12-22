using RestaurantSystem.Shared.Enums;

namespace RestaurantSystem.Shared.Contracts;

public record CrearUsuarioRequest(
    string Username,
    string Password,
    string Nombre,
    string Apellido,
    RolUsuario Rol
);

public record UsuarioDto(
    Guid Id,
    string Username,
    string Nombre,
    string Apellido,
    string NombreCompleto,
    RolUsuario Rol,
    bool Activo
);

public record CambiarRolRequest(RolUsuario Rol);

public record CambiarEstadoRequest(bool Activo);

public record ResetPasswordRequest(string NewPassword);
