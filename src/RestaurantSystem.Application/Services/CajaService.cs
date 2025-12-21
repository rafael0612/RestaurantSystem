using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Abstractions.Security;
using RestaurantSystem.Application.Dtos;
using RestaurantSystem.Application.Services.Rules;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services
{
    public interface ICajaService
    {
        Task<Guid> AbrirCajaAsync(decimal montoApertura, CancellationToken ct);
        Task RegistrarEgresoAsync(decimal monto, string motivo, CancellationToken ct);

        Task<PagoResultDto> RegistrarPagoAsync(RegistrarPagoRequest req, CancellationToken ct);

        Task CerrarCajaAsync(CerrarCajaRequest req, CancellationToken ct);

        Task<ReporteDiarioDto> GetReporteDiarioAsync(DateOnly fecha, CancellationToken ct);
    }

    public class CajaService : ICajaService
    {
        private readonly ICajaRepository _caja;
        private readonly ICuentaRepository _cuentas;
        private readonly IComandaRepository _comandas;
        private readonly IReporteRepository _reporte;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _currentUser;

        public CajaService(
            ICajaRepository caja,
            ICuentaRepository cuentas,
            IComandaRepository comandas,
            IReporteRepository reporte,
            IUnitOfWork uow,
            ICurrentUser currentUser)
        {
            _caja = caja;
            _cuentas = cuentas;
            _comandas = comandas;
            _reporte = reporte;
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Guid> AbrirCajaAsync(decimal montoApertura, CancellationToken ct)
        {
            var abierta = await _caja.GetCajaAbiertaAsync(ct);
            if (abierta is not null) return abierta.Id;

            var sesion = new CajaSesion(_currentUser.UserId, montoApertura);
            await _caja.AddCajaSesionAsync(sesion, ct);
            await _uow.SaveChangesAsync(ct);

            return sesion.Id;
        }

        public async Task RegistrarEgresoAsync(decimal monto, string motivo, CancellationToken ct)
        {
            var sesion = await _caja.GetCajaAbiertaAsync(ct) ?? throw new InvalidOperationException("Caja no está abierta.");

            var mov = new MovimientoCaja(sesion.Id, _currentUser.UserId, TipoMovimientoCaja.Egreso, monto, motivo);
            await _caja.AddMovimientoAsync(mov, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<PagoResultDto> RegistrarPagoAsync(RegistrarPagoRequest req, CancellationToken ct)
        {
            var sesion = await _caja.GetCajaAbiertaAsync(ct) ?? throw new InvalidOperationException("Caja no está abierta.");

            var cuenta = await _cuentas.GetByIdAsync(req.CuentaId, includeDetails: true, ct)
                        ?? throw new KeyNotFoundException("Cuenta no existe.");

            if (cuenta.Estado is EstadoCuenta.Cerrada or EstadoCuenta.Anulada)
                throw new InvalidOperationException("Cuenta cerrada/anulada.");

            // 1) Traer items y validar pendientes
            var itemsToPay = new List<(ComandaDetalle item, int cantidad)>();

            foreach (var detReq in req.Detalles)
            {
                var item = await _comandas.GetDetalleByIdAsync(detReq.ComandaDetalleId, ct)
                           ?? throw new KeyNotFoundException($"Item {detReq.ComandaDetalleId} no existe.");

                if (item.Anulado) throw new InvalidOperationException("No se puede pagar un item anulado.");
                if (detReq.CantidadPagada > item.CantidadPendientePago)
                    throw new InvalidOperationException("Cantidad pagada excede lo pendiente.");

                itemsToPay.Add((item, detReq.CantidadPagada));
            }

            // 2) Subtotal por detalles
            var subtotal = PagoRules.CalcularSubtotalPorDetalles(itemsToPay);

            // 3) Construir pago (detalle + métodos)
            var pago = new Pago(cuenta.Id, sesion.Id, DateTime.UtcNow);

            foreach (var (item, cantidad) in itemsToPay)
            {
                pago.AgregarDetalle(item.Id, cantidad, item.PrecioUnitario);
            }

            foreach (var m in req.Metodos)
            {
                pago.AgregarMetodo(m.Metodo, m.Monto, m.ReferenciaOperacion);
            }

            pago.ValidarConsistencia();

            // Validación adicional: por seguridad, total == subtotal
            if (pago.Total != subtotal)
                throw new InvalidOperationException("Total del pago no coincide con el subtotal.");

            // 4) Aplicar pago a items (actualiza CantidadPagada)
            foreach (var (item, cantidad) in itemsToPay)
            {
                item.AplicarPago(cantidad);
            }

            // 5) Persistir
            await _caja.AddPagoAsync(pago, ct);
            await _uow.SaveChangesAsync(ct);

            // 6) Si ya está todo pagado, cerrar cuenta automáticamente (regla MVP)
            if (cuenta.SaldoPendiente <= 0)
                cuenta.Cerrar();

            await _uow.SaveChangesAsync(ct);

            return new PagoResultDto(pago.Id, pago.Total, pago.PagadoEn);
        }

        public async Task CerrarCajaAsync(CerrarCajaRequest req, CancellationToken ct)
        {
            var sesion = await _caja.GetCajaAbiertaAsync(ct) ?? throw new InvalidOperationException("Caja no está abierta.");

            // El cálculo exacto de efectivo esperado se hace mejor en Infra con query agregada.
            // Aquí lo dejamos como placeholder: en Infra lo implementaremos y lo pasaremos.
            //decimal efectivoEsperado = sesion.MontoApertura; // TODO: reemplazar con cálculo real
            decimal efectivoEsperado = await _caja.CalcularEfectivoEsperadoAsync(sesion.Id, ct); // TODO: reemplazar con cálculo real

            sesion.Cerrar(req.MontoContado, efectivoEsperado, req.MotivoDiferencia);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<ReporteDiarioDto> GetReporteDiarioAsync(DateOnly fecha, CancellationToken ct)
        {
            var (ventasTotal, ventasEfectivo, ventasYape, ventasPlin, egresos, costosEstimados, cantidadPedidos, top) =
                await _reporte.GetReporteDiarioAsync(fecha, ct);

            var ganancia = ventasTotal - costosEstimados;

            return new ReporteDiarioDto(
                fecha,
                ventasTotal,
                ventasEfectivo,
                ventasYape,
                ventasPlin,
                egresos,
                costosEstimados,
                ganancia,
                cantidadPedidos,
                top.Select(x => new ProductoVendidoDto(x.productoId, x.nombre, x.cantidad, x.monto)).ToList()
            );
        }
    }
}
