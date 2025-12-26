using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Services;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.API.Controllers
{
    [ApiController]
    [Route("api/mesero")]
    [Authorize(Roles = "Mesero,Admin")]
    public class MeseroController : ControllerBase
    {
        private readonly IMeseroService _mesero;

        public MeseroController(IMeseroService mesero) => _mesero = mesero;

        [HttpGet("mesas")]
        [ProducesResponseType(typeof(List<MesaResumenDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<MesaResumenDto>>> GetMesas(CancellationToken ct)
        {
            var list = await _mesero.GetMesasAsync(ct);
            return Ok(list);
        }

        [HttpGet("activas-para-llevar")]
        [ProducesResponseType(typeof(List<CuentasActivasParaLlevarDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CuentasActivasParaLlevarDto>>> GetCuentasActivasParaLlevar(CancellationToken ct)
        {
            var list = await _mesero.GetCuentasActivasParaLlevarAsync(ct);
            return Ok(list);
        }

        [HttpPost("cuentas")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AbrirCuenta([FromBody] AbrirCuentaRequest req, CancellationToken ct)
        {
            var cuentaId = await _mesero.AbrirCuentaAsync(req, ct);
            return Ok(cuentaId);
        }

        [HttpPost("cuentas/{cuentaId:guid}/solicitar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SolicitarCuenta(Guid cuentaId, CancellationToken ct)
        {
            await _mesero.SolicitarCuentaAsync(cuentaId, ct);
            return NoContent();
        }

        [HttpGet("cuentas/{cuentaId:guid}")]
        [ProducesResponseType(typeof(CuentaDetalleDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CuentaDetalleDto>> GetCuentaDetalle(Guid cuentaId, CancellationToken ct)
        {
            var dto = await _mesero.GetCuentaDetalleAsync(cuentaId, ct);
            return Ok(dto);
        }

        [HttpPost("cuentas/{cuentaId:guid}/comandas")]
        [ProducesResponseType(typeof(CrearComandaResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CrearComandaResponse>> CrearComanda(Guid cuentaId, CancellationToken ct)
        {
            var dto = await _mesero.CrearComandaAsync(cuentaId, ct);
            return Ok(dto);
        }

        [HttpPost("comandas/{comandaId:guid}/items")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AgregarItem(Guid comandaId, [FromBody] AgregarItemRequest req, CancellationToken ct)
        {
            var detalleId = await _mesero.AgregarItemAsync(comandaId, req, ct);
            return Ok(detalleId);
        }

        [HttpPost("comandas/{comandaId:guid}/enviar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> EnviarACocina(Guid comandaId, [FromBody] EnviarACocinaRequest req, CancellationToken ct)
        {
            await _mesero.EnviarComandaACocinaAsync(comandaId, req.ImprimirComanda, ct);
            return NoContent();
        }
    }
}
