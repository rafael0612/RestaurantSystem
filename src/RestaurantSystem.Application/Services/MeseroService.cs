using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Abstractions.Security;
using RestaurantSystem.Application.Dtos;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services
{
    public interface IMeseroService
    {
        Task<List<MesaResumenDto>> GetMesasAsync(CancellationToken ct);
        Task<Guid> AbrirCuentaAsync(AbrirCuentaRequest req, CancellationToken ct);
        Task SolicitarCuentaAsync(Guid cuentaId, CancellationToken ct);

        Task<CrearComandaResponse> CrearComandaAsync(Guid cuentaId, CancellationToken ct);
        Task<Guid> AgregarItemAsync(Guid comandaId, AgregarItemRequest req, CancellationToken ct);
        Task EnviarComandaACocinaAsync(Guid comandaId, bool imprimir, CancellationToken ct);

        Task<CuentaDetalleDto> GetCuentaDetalleAsync(Guid cuentaId, CancellationToken ct);
    }

    public class MeseroService : IMeseroService
    {
        private readonly IMesaRepository _mesas;
        private readonly ICuentaRepository _cuentas;
        private readonly IComandaRepository _comandas;
        private readonly IProductoRepository _productos;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _currentUser;

        public MeseroService(
            IMesaRepository mesas,
            ICuentaRepository cuentas,
            IComandaRepository comandas,
            IProductoRepository productos,
            IUnitOfWork uow,
            ICurrentUser currentUser)
        {
            _mesas = mesas;
            _cuentas = cuentas;
            _comandas = comandas;
            _productos = productos;
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<List<MesaResumenDto>> GetMesasAsync(CancellationToken ct)
        {
            var list = await _mesas.GetAllAsync(ct);

            // Para mostrar cuenta activa por mesa, consultamos por mesa (4 mesas => costo OK)
            var result = new List<MesaResumenDto>(list.Count);
            foreach (var m in list.OrderBy(x => x.Nombre))
            {
                var cuentaActiva = await _cuentas.GetCuentaActivaPorMesaAsync(m.Id, ct);
                result.Add(new MesaResumenDto(m.Id, m.Nombre, m.Estado, m.NroPersonas, cuentaActiva?.Id));
            }
            return result;
        }

        public async Task<Guid> AbrirCuentaAsync(AbrirCuentaRequest req, CancellationToken ct)
        {
            if (req.Tipo == TipoCuenta.Salon)
            {
                if (!req.MesaId.HasValue) throw new InvalidOperationException("MesaId requerido para salón.");

                // Si ya hay una cuenta activa en la mesa, devolvemos esa cuenta
                var existente = await _cuentas.GetCuentaActivaPorMesaAsync(req.MesaId.Value, ct);
                if (existente is not null) return existente.Id;
            }

            var cuenta = new Cuenta(req.Tipo, req.MesaId, _currentUser.UserId, req.ObservacionGeneral);

            await _cuentas.AddAsync(cuenta, ct);
            await _uow.SaveChangesAsync(ct);

            return cuenta.Id;
        }

        public async Task SolicitarCuentaAsync(Guid cuentaId, CancellationToken ct)
        {
            var cuenta = await _cuentas.GetByIdAsync(cuentaId, includeDetails: false, ct)
                        ?? throw new KeyNotFoundException("Cuenta no existe.");

            cuenta.SolicitarCuenta();
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<CrearComandaResponse> CrearComandaAsync(Guid cuentaId, CancellationToken ct)
        {
            var cuenta = await _cuentas.GetByIdAsync(cuentaId, includeDetails: true, ct)
                        ?? throw new KeyNotFoundException("Cuenta no existe.");

            if (cuenta.Estado is EstadoCuenta.Cerrada or EstadoCuenta.Anulada)
                throw new InvalidOperationException("Cuenta cerrada/anulada.");

            var nextSeq = cuenta.Comandas.Count == 0 ? 1 : cuenta.Comandas.Max(x => x.NumeroSecuencia) + 1;

            var comanda = new Comanda(cuentaId, _currentUser.UserId, nextSeq);
            await _comandas.AddAsync(comanda, ct);

            // mantener agregado lógico (opcional)
            cuenta.ReabrirDesdePorCobrar();
            cuenta.GetType(); // noop (solo para enfatizar que está cargada)

            await _uow.SaveChangesAsync(ct);

            return new CrearComandaResponse(comanda.Id, comanda.NumeroSecuencia);
        }

        public async Task<Guid> AgregarItemAsync(Guid comandaId, AgregarItemRequest req, CancellationToken ct)
        {
            var comanda = await _comandas.GetByIdAsync(comandaId, includeDetails: true, ct)
                         ?? throw new KeyNotFoundException("Comanda no existe.");

            var producto = await _productos.GetByIdAsync(req.ProductoId, ct)
                          ?? throw new KeyNotFoundException("Producto no existe.");

            if (!producto.Activo) throw new InvalidOperationException("Producto inactivo.");

            comanda.AgregarItem(producto.Id, req.Cantidad, producto.Precio, producto.CostoEstandar, req.Observacion);

            await _uow.SaveChangesAsync(ct);

            // El último detalle creado (en EF se guardará con Id)
            var last = comanda.Detalles.Last();
            return last.Id;
        }

        public async Task EnviarComandaACocinaAsync(Guid comandaId, bool imprimir, CancellationToken ct)
        {
            var comanda = await _comandas.GetByIdAsync(comandaId, includeDetails: true, ct)
                         ?? throw new KeyNotFoundException("Comanda no existe.");

            comanda.EnviarACocina();
            await _uow.SaveChangesAsync(ct);

            // impresión se ejecutará desde capa API/Infra cuando agreguemos IPrinterService
            _ = imprimir;
        }

        public async Task<CuentaDetalleDto> GetCuentaDetalleAsync(Guid cuentaId, CancellationToken ct)
        {
            var cuenta = await _cuentas.GetByIdAsync(cuentaId, includeDetails: true, ct)
                        ?? throw new KeyNotFoundException("Cuenta no existe.");

            return new CuentaDetalleDto(
                cuenta.Id,
                cuenta.Tipo,
                cuenta.Estado,
                cuenta.MesaId,
                MesaNombre: null,
                cuenta.AperturaEn,
                cuenta.TotalConsumido,
                cuenta.TotalPagado,
                cuenta.SaldoPendiente
            );
        }
    }
}
