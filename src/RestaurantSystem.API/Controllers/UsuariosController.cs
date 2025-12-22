using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Services;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize(Roles = "Admin")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _users;

        public UsuariosController(IUsuarioService users) => _users = users;

        [HttpPost]
        public async Task<ActionResult<Guid>> Crear([FromBody] CrearUsuarioRequest req, CancellationToken ct)
        {
            var id = await _users.CrearAsync(req, ct);
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<List<UsuarioDto>>> Listar(CancellationToken ct)
        {
            var list = await _users.ListAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UsuarioDto>> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _users.GetByIdAsync(id, ct);
            return Ok(dto);
        }

        [HttpPatch("{id:guid}/rol")]
        public async Task<IActionResult> CambiarRol(Guid id, [FromBody] CambiarRolRequest req, CancellationToken ct)
        {
            await _users.CambiarRolAsync(id, req, ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/estado")]
        public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoRequest req, CancellationToken ct)
        {
            await _users.CambiarEstadoAsync(id, req, ct);
            return NoContent();
        }

        [HttpPut("{id:guid}/password")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest req, CancellationToken ct)
        {
            await _users.ResetPasswordAsync(id, req, ct);
            return NoContent();
        }
    }
}
