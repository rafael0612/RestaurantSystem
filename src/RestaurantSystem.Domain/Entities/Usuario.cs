using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Usuario : AggregateRoot
    {
        private Usuario() { } // Para EF

        public Usuario(string nombre, string username, string apellido, string passwordHash, RolUsuario rol)
        {
            Guard.AgainstNullOrEmpty(nombre, "Nombre es requerido.");
            Guard.AgainstNullOrEmpty(apellido, "Apellido es requerido.");
            Guard.AgainstNullOrEmpty(username, "Username es requerido.");
            Guard.AgainstNullOrEmpty(passwordHash, "PasswordHash es requerido.");

            Nombre = nombre.Trim();
            Apellido = apellido.Trim();
            Username = username.Trim();
            PasswordHash = passwordHash;
            NombreCompleto = BuildFullName(Nombre, Apellido);
            Rol = rol;
            Activo = true;
        }

        public string Nombre { get; private set; } = default!;
        public string Apellido { get; private set; } = default!;
        // Derivado (persistido): "Nombre Apellido"
        public string NombreCompleto { get; private set; } = default!;
        public string Username { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public RolUsuario Rol { get; private set; }
        public bool Activo { get; private set; }

        public void Desactivar() => Activo = false;
        public void Activar() => Activo = true;

        public void CambiarNombre(string newNombre)
        {
            Guard.AgainstNullOrEmpty(newNombre, "Nombre nuevo es requerido.");
            Nombre = newNombre;
            NombreCompleto = BuildFullName(Nombre, Apellido);
        }

        public void CambiarApellido(string newApellido)
        {
            Guard.AgainstNullOrEmpty(newApellido, "Apellido nuevo es requerido.");
            Apellido = newApellido;
            NombreCompleto = BuildFullName(Nombre, Apellido);
        }
        public void CambiarPasswordHash(string newHash)
        {
            Guard.AgainstNullOrEmpty(newHash, "PasswordHash nuevo es requerido.");
            PasswordHash = newHash;
        }
        public void CambiarRol(RolUsuario newRol) => Rol = newRol;
        private static string BuildFullName(string nombre, string apellido)
        => $"{nombre} {apellido}".Trim();
    }
}
