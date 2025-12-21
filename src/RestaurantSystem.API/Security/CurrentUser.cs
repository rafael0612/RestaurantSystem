using RestaurantSystem.Application.Abstractions.Security;
using System.Security.Claims;

namespace RestaurantSystem.API.Security
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUser(IHttpContextAccessor http) => _http = http;

        public Guid UserId
        {
            get
            {
                var user = _http.HttpContext?.User;
                if (user is null) return Guid.Empty;

                var idStr = user.FindFirst("userId")?.Value
                            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return Guid.TryParse(idStr, out var id) ? id : Guid.Empty;
            }
        }

        public string Role
        {
            get
            {
                var user = _http.HttpContext?.User;
                if (user is null) return string.Empty;

                return user.FindFirst(ClaimTypes.Role)?.Value
                    ?? user.FindFirst("role")?.Value
                    ?? string.Empty;
            }
        }
    }
}
