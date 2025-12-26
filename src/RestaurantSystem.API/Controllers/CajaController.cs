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

        //[HttpGet("sesion")]
        //[ProducesResponseType(typeof(CajaSesionDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> GetSesionActual(CancellationToken ct)
        //{
        //    var dto = await _caja.GetSesionAbiertaAsync(ct);
        //    return dto is null ? NoContent() : Ok(dto);
        //}
        [HttpGet("sesion")]
        [ProducesResponseType(typeof(CajaSesionDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CajaSesionDto?>> GetSesionActual(CancellationToken ct)
            => Ok(await _caja.GetSesionActualAsync(ct));

        //[HttpPost("abrir")]
        //[ProducesResponseType(typeof(CajaSesionDto), StatusCodes.Status200OK)]
        //public async Task<IActionResult> Abrir([FromBody] AbrirCajaRequest req, CancellationToken ct)
        //    => Ok(await _caja.AbrirCajaAsync(req.MontoApertura, ct));
        [HttpPost("abrir")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> Abrir([FromBody] AbrirCajaRequest req, CancellationToken ct)
            => Ok(await _caja.AbrirCajaAsync(req, ct));

        [HttpPost("egresos")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RegistrarEgreso([FromBody] RegistrarEgresoRequest req, CancellationToken ct)
        {
            await _caja.RegistrarEgresoAsync(req, ct);
            return NoContent();
        }

        [HttpGet("cuentas")]
        [ProducesResponseType(typeof(List<CuentaPorCobrarDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CuentasPorCobrar(CancellationToken ct)
            => Ok(await _caja.ListarCuentasPorCobrarAsync(ct));

        [HttpGet("cuentas/por-cobrar")]
        [ProducesResponseType(typeof(List<CuentaPorCobrarDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CuentaPorCobrarDto>>> ListarPorCobrar(CancellationToken ct)
        => Ok(await _caja.ListarCuentasPorCobrarAsync(ct));

        [HttpGet("cuentas/{cuentaId:guid}/cobro")]
        [ProducesResponseType(typeof(CuentaCobroDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CuentaCobroDto>> GetCuentaCobro(Guid cuentaId, CancellationToken ct)
            => Ok(await _caja.GetCuentaCobroAsync(cuentaId, ct));

        //[HttpGet("cuentas/{cuentaId:guid}")]
        //[ProducesResponseType(typeof(CuentaCobroDto), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetCuentaCobro(Guid cuentaId, CancellationToken ct)
        //    => Ok(await _caja.GetCuentaCobroAsync(cuentaId, ct));

        //[HttpPost("pagos")]
        //[ProducesResponseType(typeof(RegistrarPagoResponse), StatusCodes.Status200OK)]
        //public async Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoRequest req, CancellationToken ct)
        //    => Ok(await _caja.RegistrarPagoAsync(req, ct));
        [HttpPost("pagos")]
        [ProducesResponseType(typeof(RegistrarPagoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RegistrarPagoResponse>> RegistrarPago([FromBody] RegistrarPagoRequest req, CancellationToken ct)
            => Ok(await _caja.RegistrarPagoAsync(req, ct));

        //[HttpPost("cerrar")]
        //[ProducesResponseType(typeof(CerrarCajaResponse), StatusCodes.Status200OK)]
        //public async Task<IActionResult> CerrarCaja([FromBody] CerrarCajaRequest req, CancellationToken ct)
        //    => Ok(await _caja.CerrarCajaAsync(req, ct));
        [HttpPost("cerrar")]
        [ProducesResponseType(typeof(CerrarCajaResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CerrarCajaResponse>> Cerrar([FromBody] CerrarCajaRequest req, CancellationToken ct)
            => Ok(await _caja.CerrarCajaAsync(req, ct));

        //[HttpGet("reporte")]
        //[ProducesResponseType(typeof(ReporteDiarioCajaDto), StatusCodes.Status200OK)]
        //public async Task<IActionResult> Reporte([FromQuery] DateOnly? fecha, CancellationToken ct)
        //    => Ok(await _caja.GetReporteDiarioCajaAsync(fecha ?? DateOnly.FromDateTime(DateTime.Today), ct));

        [HttpGet("reporte")]
        [ProducesResponseType(typeof(ReporteDiarioCajaDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ReporteDiarioCajaDto>> Reporte([FromQuery] DateOnly fecha, CancellationToken ct)
        => Ok(await _caja.GetReporteDiarioCajaAsync(fecha, ct));
    }
}
