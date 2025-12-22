using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Services;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            try
            {
                var resp = await _auth.LoginAsync(req, ct);
                return Ok(resp);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
            }
        }

        // Útil para probar JWT rápidamente
        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            return Ok(new
            {
                userId = User.FindFirst("userId")?.Value,
                username = User.FindFirst("username")?.Value,
                role = User.FindFirst("role")?.Value
            });
        }
    }
}
