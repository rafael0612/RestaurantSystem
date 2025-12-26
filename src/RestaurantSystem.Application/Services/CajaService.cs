using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Abstractions.Security;
using RestaurantSystem.Application.Common;
using RestaurantSystem.Application.Services.Rules;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Shared.Contracts;
using D = RestaurantSystem.Domain.Enums;
using S = RestaurantSystem.Shared.Enums;

namespace RestaurantSystem.Application.Services
{
    public interface ICajaService
    {
        Task<CajaSesionDto?> GetSesionActualAsync(CancellationToken ct);
        Task<Guid> AbrirCajaAsync(AbrirCajaRequest req, CancellationToken ct);
        Task RegistrarEgresoAsync(RegistrarEgresoRequest req, CancellationToken ct);

        Task<List<CuentaPorCobrarDto>> ListarCuentasPorCobrarAsync(CancellationToken ct);
        Task<CuentaCobroDto> GetCuentaCobroAsync(Guid cuentaId, CancellationToken ct);

        Task<RegistrarPagoResponse> RegistrarPagoAsync(RegistrarPagoRequest req, CancellationToken ct);

        Task<CerrarCajaResponse> CerrarCajaAsync(CerrarCajaRequest req, CancellationToken ct);
        Task<ReporteDiarioCajaDto> GetReporteDiarioCajaAsync(DateOnly fecha, CancellationToken ct);
    }

    public class CajaService : ICajaService
    {
        private readonly ICajaRepository _caja;
        private readonly ICuentaRepository _cuentas;
        private readonly IComandaRepository _comandas;
        private readonly IReporteRepository _reporte;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _currentUser;
        private readonly IMesaRepository _mesas;

        public CajaService(
            ICajaRepository caja,
            ICuentaRepository cuentas,
            IComandaRepository comandas,
            IReporteRepository reporte,
            IUnitOfWork uow,
            ICurrentUser currentUser,
            IMesaRepository mesas)
        {
            _caja = caja;
            _cuentas = cuentas;
            _comandas = comandas;
            _reporte = reporte;
            _uow = uow;
            _currentUser = currentUser;
            _mesas = mesas;
        }
        public async Task<CajaSesionDto?> GetSesionActualAsync(CancellationToken ct)
        {
            var sesion = await _caja.GetSesionAbiertaPorUsuarioAsync(_currentUser.UserId, ct);
            if (sesion is null) return null;

            // Para POS es MUY útil mostrar “efectivo esperado” mientras está abierta.
            var esperado = await _caja.CalcularEfectivoEsperadoAsync(sesion.Id, ct);

            return new CajaSesionDto(
                CajaSesionId: sesion.Id,
                AperturaEn: sesion.AperturaEn,
                MontoApertura: sesion.MontoApertura,
                Abierta: sesion.Estado == EstadoCajaSesion.Abierta,
                CierreEn: sesion.CierreEn,
                MontoCierreContado: sesion.MontoCierreContado,
                EfectivoEsperado: esperado,
                Diferencia: sesion.Diferencia
            );
        }
        public async Task<Guid> AbrirCajaAsync(AbrirCajaRequest req, CancellationToken ct)
        {
            var abierta = await _caja.GetSesionAbiertaPorUsuarioAsync(_currentUser.UserId, ct);
            if (abierta is not null) return abierta.Id;

            var sesion = new CajaSesion(_currentUser.UserId, req.MontoApertura);
            await _caja.AddCajaSesionAsync(sesion, ct);
            await _uow.SaveChangesAsync(ct);
            return sesion.Id;
        }

        public async Task RegistrarEgresoAsync(RegistrarEgresoRequest req, CancellationToken ct)
        {
            var sesion = await _caja.GetSesionAbiertaPorUsuarioAsync(_currentUser.UserId, ct)
                        ?? throw new InvalidOperationException("No hay caja abierta para el usuario.");

            if (req.Monto <= 0) throw new InvalidOperationException("Monto debe ser > 0.");
            if (string.IsNullOrWhiteSpace(req.Motivo)) throw new InvalidOperationException("Motivo es requerido.");

            var mov = new MovimientoCaja(sesion.Id, _currentUser.UserId, D.TipoMovimientoCaja.Egreso, req.Monto, req.Motivo);
            await _caja.AddMovimientoAsync(mov, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<List<CuentaPorCobrarDto>> ListarCuentasPorCobrarAsync(CancellationToken ct)
            => _caja.ListarCuentasPorCobrarAsync(ct);

        public Task<CuentaCobroDto> GetCuentaCobroAsync(Guid cuentaId, CancellationToken ct)
            => _caja.GetCuentaCobroAsync(cuentaId, ct);

        public async Task<RegistrarPagoResponse> RegistrarPagoAsync(RegistrarPagoRequest req, CancellationToken ct)
        {
            if (req is null) throw new ArgumentNullException(nameof(req));

            // 1) Caja abierta (del usuario logueado)
            var sesion = await _caja.GetSesionAbiertaPorUsuarioAsync(_currentUser.UserId, ct) 
                        ?? throw new InvalidOperationException("No hay una caja abierta para el usuario.");

            // 2) Cargar cuenta con detalles necesarios para cobrar
            var cuenta = await _cuentas.GetByIdAsync(req.CuentaId, includeDetails: true, ct)
                        ?? throw new KeyNotFoundException("Cuenta no existe.");

            // EstadoCuenta es Domain enum en la entidad Cuenta
            if (cuenta.Estado is D.EstadoCuenta.Cerrada or D.EstadoCuenta.Anulada)
                throw new InvalidOperationException("Cuenta cerrada/anulada.");

            // exigir PorCobrar antes de pagar
            if (cuenta.Estado != D.EstadoCuenta.PorCobrar)
                throw new InvalidOperationException("La cuenta debe estar en 'PorCobrar' antes de pagar.");

            // 3) Validaciones básicas de request
            if (req.Detalles is null || req.Detalles.Count == 0)
                throw new InvalidOperationException("Debe seleccionar al menos un ítem.");

            if (req.Metodos is null || req.Metodos.Count == 0)
                throw new InvalidOperationException("Debe registrar al menos un método de pago.");

            // 4) Build lookup de detalles que pertenecen a la cuenta (seguridad)
            //    (asume que includeDetails trae comandas + detalles)
            var detallesDeLaCuenta = cuenta.Comandas
                .SelectMany(c => c.Detalles)
                .ToDictionary(d => d.Id, d => d);

            // 5) Agrupar por detalleId por si el UI envía repetidos
            var detallesAgrupados = req.Detalles
                .GroupBy(d => d.ComandaDetalleId)
                .Select(g => new { DetalleId = g.Key, Cantidad = g.Sum(x => x.CantidadPagada) })
                .ToList();

            // 6) Traer items y validar pendientes
            var itemsToPay = new List<(ComandaDetalle item, int cantidad)>();

            foreach (var detReq in req.Detalles)
            {
                if (detReq.CantidadPagada <= 0)
                    throw new InvalidOperationException("Cantidad pagada debe ser mayor a 0.");

                if (!detallesDeLaCuenta.TryGetValue(detReq.ComandaDetalleId, out var item))
                    throw new InvalidOperationException("El ítem no pertenece a la cuenta (o no existe).");

                if (item.Anulado)
                    throw new InvalidOperationException("No se puede pagar un ítem anulado.");

                if (detReq.CantidadPagada > item.CantidadPendientePago)
                    throw new InvalidOperationException("Cantidad pagada excede lo pendiente del ítem.");

                // Prohibir pagar si no está Listo/Entregado:
                if (item.EstadoCocina is D.EstadoCocinaItem.Pendiente or D.EstadoCocinaItem.Preparando)
                    throw new InvalidOperationException("No se puede pagar un ítem que aún no está listo.");


                //var item = await _comandas.GetDetalleByIdAsync(detReq.ComandaDetalleId, ct)
                //           ?? throw new KeyNotFoundException($"Item {detReq.ComandaDetalleId} no existe.");

                //if (item.Anulado) throw new InvalidOperationException("No se puede pagar un item anulado.");
                //if (detReq.CantidadPagada <= 0) throw new InvalidOperationException("Cantidad pagada debe ser > 0.");
                //if (detReq.CantidadPagada > item.CantidadPendientePago)
                //    throw new InvalidOperationException("Cantidad pagada excede lo pendiente.");

                itemsToPay.Add((item, detReq.CantidadPagada));
            }

            //if (itemsToPay.Count == 0)
            //    throw new InvalidOperationException("Debe seleccionar al menos un ítem.");

            // 7) Subtotal por detalles
            static decimal R2(decimal x) => Math.Round(x, 2, MidpointRounding.AwayFromZero);

            
            var subtotal = R2(PagoRules.CalcularSubtotalPorDetalles(itemsToPay));

            // 8) Validar métodos (sumas)
            foreach (var m in req.Metodos)
            {
                if (m.Monto <= 0)
                    throw new InvalidOperationException("Monto de método debe ser mayor a 0.");
            }

            var totalMetodos = R2(req.Metodos.Sum(m => m.Monto));

            if (totalMetodos != subtotal)
                throw new InvalidOperationException($"La suma de métodos ({totalMetodos}) debe igualar el total ({subtotal}).");


            // 9) Construir pago (detalle + métodos)
            var pago = new Pago(cuenta.Id, sesion.Id, DateTime.UtcNow);

            foreach (var (item, cantidad) in itemsToPay)
                pago.AgregarDetalle(item.Id, cantidad, item.PrecioUnitario);

            foreach (var m in req.Metodos)
                pago.AgregarMetodo(m.Metodo.ToDomain(), m.Monto, m.ReferenciaOperacion);

            pago.ValidarConsistencia();

            // Validación adicional: por seguridad, total == subtotal
            if (pago.Total != subtotal)
                throw new InvalidOperationException("Total del pago no coincide con el subtotal.");

            // 10) Aplicar pago a items (actualiza CantidadPagada)
            foreach (var (item, cantidad) in itemsToPay)
                item.AplicarPago(cantidad);

            // 11) Persistir
            await _caja.AddPagoAsync(pago, ct);

            // 12) Si ya está todo pagado => cerrar cuenta + liberar mesa
            if (cuenta.SaldoPendiente <= 0)
            {
                cuenta.Cerrar();

                // Si es salón, liberar mesa
                if (cuenta.Tipo == D.TipoCuenta.Salon && cuenta.MesaId is not null)
                {
                    var mesa = await _mesas.GetByIdAsync(cuenta.MesaId.Value, ct)
                               ?? throw new InvalidOperationException("Mesa asociada no existe.");

                    mesa.MarcarLibre(); // Dejar Libre + limpiar CuentaActivaId si tu dominio lo maneja así
                }
            }

            await _uow.SaveChangesAsync(ct);

            // PagoResultDto es Shared.Contracts, no requiere mapping
            return new RegistrarPagoResponse(
                PagoId: pago.Id, 
                TotalPagado: pago.Total, 
                SaldoPendiente: cuenta.SaldoPendiente
            );
        }

        public async Task<CerrarCajaResponse> CerrarCajaAsync(CerrarCajaRequest req, CancellationToken ct)
        {
            var sesion = await _caja.GetSesionAbiertaPorUsuarioAsync(_currentUser.UserId, ct)
                        ?? throw new InvalidOperationException("No hay caja abierta para el usuario.");

            var esperado = await _caja.CalcularEfectivoEsperadoAsync(sesion.Id, ct);

            sesion.Cerrar(req.MontoContado, esperado, req.MotivoDiferencia);
            await _uow.SaveChangesAsync(ct);

            return new CerrarCajaResponse(
                CajaSesionId: sesion.Id,
                EfectivoEsperado: esperado,
                MontoContado: req.MontoContado,
                Diferencia: req.MontoContado - esperado
            );
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
        public Task<ReporteDiarioCajaDto> GetReporteDiarioCajaAsync(DateOnly fecha, CancellationToken ct)
            => _caja.GetReporteDiarioCajaAsync(fecha, ct);
    }
}
