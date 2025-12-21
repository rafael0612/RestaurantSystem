using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Services;
using RestaurantSystem.Shared.Contracts;
using RestaurantSystem.Shared.Enums;

namespace RestaurantSystem.API.Controllers
{
    [ApiController]
    [Route("api/cocina")]
    [Authorize(Roles = "Cocinero,Admin")]
    public class CocinaController : ControllerBase
    {
        private readonly ICocinaService _cocina;

        public CocinaController(ICocinaService cocina) => _cocina = cocina;

        [HttpGet("kds")]
        [ProducesResponseType(typeof(List<KdsCardDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<KdsCardDto>>> GetKds([FromQuery] EstadoCocinaItem? estado, CancellationToken ct)
        {
            var cards = await _cocina.GetKdsAsync(estado, ct);
            return Ok(cards);
        }

        [HttpPatch("items/{detalleId:guid}/estado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CambiarEstado(Guid detalleId, [FromBody] EstadoCocinaItem nuevoEstado, CancellationToken ct)
        {
            await _cocina.CambiarEstadoItemAsync(detalleId, nuevoEstado, ct);
            return NoContent();
        }
    }
}
