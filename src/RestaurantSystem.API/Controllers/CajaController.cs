using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Services;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.API.Controllers
{
    [ApiController]
    [Route("api/caja")]
    [Authorize(Roles = "Caja,Admin")]
    public class CajaController : ControllerBase
    {
        private readonly ICajaService _caja;

        public CajaController(ICajaService caja) => _caja = caja;

        [HttpPost("abrir")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AbrirCaja([FromBody] AbrirCajaRequest req, CancellationToken ct)
        {
            var cajaId = await _caja.AbrirCajaAsync(req.MontoApertura, ct);
            return Ok(cajaId);
        }

        [HttpPost("egreso")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RegistrarEgreso([FromBody] RegistrarEgresoRequest req, CancellationToken ct)
        {
            await _caja.RegistrarEgresoAsync(req.Monto, req.Motivo, ct);
            return NoContent();
        }

        [HttpPost("pagos")]
        [ProducesResponseType(typeof(PagoResultDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagoResultDto>> RegistrarPago([FromBody] RegistrarPagoRequest req, CancellationToken ct)
        {
            var dto = await _caja.RegistrarPagoAsync(req, ct);
            return Ok(dto);
        }

        [HttpPost("cerrar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CerrarCaja([FromBody] CerrarCajaRequest req, CancellationToken ct)
        {
            await _caja.CerrarCajaAsync(req, ct);
            return NoContent();
        }

        [HttpGet("reporte-diario")]
        [ProducesResponseType(typeof(ReporteDiarioDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ReporteDiarioDto>> ReporteDiario([FromQuery] DateOnly fecha, CancellationToken ct)
        {
            var dto = await _caja.GetReporteDiarioAsync(fecha, ct);
            return Ok(dto);
        }
    }
}
