using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class User : AggregateRoot
    {
        private User() { } // Para EF

        public User(string nombre, string username, string passwordHash, RolUsuario rol)
        {
            Guard.AgainstNullOrEmpty(nombre, "Nombre es requerido.");
            Guard.AgainstNullOrEmpty(username, "Username es requerido.");
            Guard.AgainstNullOrEmpty(passwordHash, "PasswordHash es requerido.");

            Nombre = nombre.Trim();
            Username = username.Trim();
            PasswordHash = passwordHash;
            Rol = rol;
            Activo = true;
        }

        public string Nombre { get; private set; } = default!;
        public string Username { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public RolUsuario Rol { get; private set; }
        public bool Activo { get; private set; }

        public void Desactivar() => Activo = false;
        public void Activar() => Activo = true;

        public void CambiarPasswordHash(string newHash)
        {
            Guard.AgainstNullOrEmpty(newHash, "PasswordHash nuevo es requerido.");
            PasswordHash = newHash;
        }
    }
}
