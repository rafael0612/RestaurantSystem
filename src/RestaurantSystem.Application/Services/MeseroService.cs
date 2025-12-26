using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Abstractions.Security;
using RestaurantSystem.Application.Common;
using RestaurantSystem.Shared.Contracts;
using RestaurantSystem.Domain.Entities;
using D = RestaurantSystem.Domain.Enums;
using S = RestaurantSystem.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace RestaurantSystem.Application.Services
{
    public interface IMeseroService
    {
        Task<List<MesaResumenDto>> GetMesasAsync(CancellationToken ct);
        Task<List<CuentasActivasParaLlevarDto>> GetCuentasActivasParaLlevarAsync(CancellationToken ct);
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
                result.Add(new MesaResumenDto(m.Id, m.Nombre, m.Estado.ToShared(), m.NroPersonas, cuentaActiva?.Id));
            }
            return result;
        }

        public async Task<List<CuentasActivasParaLlevarDto>> GetCuentasActivasParaLlevarAsync(CancellationToken ct)
        {
            // Obtener todas las cuentas activas para llevar
            var allCuentas = await _cuentas.GetAllCuentasActivasParaLlevarAsync(ct);
            // Mapear a DTOs
            var result = new List<CuentasActivasParaLlevarDto>(allCuentas.Count);
            foreach (var c in allCuentas)
            {
                result.Add(new CuentasActivasParaLlevarDto(
                    CuentaActivaId: c.Id,
                    AperturaEn: c.AperturaEn
                ));
            }
            return result;
        }

        public async Task<Guid> AbrirCuentaAsync(AbrirCuentaRequest req, CancellationToken ct)
        {
            var tipoDomain = req.Tipo.ToDomain();
            if (tipoDomain == D.TipoCuenta.Salon)
            {
                if (!req.MesaId.HasValue) throw new InvalidOperationException("Mesa requerido para salón.");

                // Si ya hay una cuenta activa en la mesa, devolvemos esa cuenta
                var existente = await _cuentas.GetCuentaActivaPorMesaAsync(req.MesaId.Value, ct);
                if (existente is not null) return existente.Id;

                //Ocupar mesa si es cuenta salón
                var mesa = await _mesas.GetByIdAsync(req.MesaId.Value, ct)
                    ?? throw new KeyNotFoundException("Mesa no existe");

                mesa.MarcarOcupada(req.NroPersonas);
            }

            var cuenta = new Cuenta(req.Tipo.ToDomain(), req.MesaId, _currentUser.UserId, req.ObservacionGeneral);

            await _cuentas.AddAsync(cuenta, ct);
            await _uow.SaveChangesAsync(ct);

            return cuenta.Id;
        }

        public async Task SolicitarCuentaAsync(Guid cuentaId, CancellationToken ct)
        {
            var cuenta = await _cuentas.GetByIdAsync(cuentaId, includeDetails: false, ct)
                        ?? throw new KeyNotFoundException("Cuenta no existe.");

            if (cuenta.Estado != D.EstadoCuenta.Abierta)
                throw new InvalidOperationException("Solo se puede solicitar cobro desde una cuenta Abierta.");

            cuenta.SolicitarCuenta();

            // Si es salón, la mesa pasa a "PorCobrar"
            if (cuenta.Tipo == D.TipoCuenta.Salon && cuenta.MesaId is not null)
            {
                var mesa = await _mesas.GetByIdAsync(cuenta.MesaId.Value, ct)
                           ?? throw new KeyNotFoundException("Mesa no existe.");

                mesa.MarcarPorCobrar();
            }

            await _uow.SaveChangesAsync(ct);
        }

        public async Task<CrearComandaResponse> CrearComandaAsync(Guid cuentaId, CancellationToken ct)
        {
            var cuenta = await _cuentas.GetByIdAsync(cuentaId, includeDetails: true, ct)
                        ?? throw new KeyNotFoundException("Cuenta no existe.");

            // EstadoCuenta aquí es Domain enum
            if (cuenta.Estado is D.EstadoCuenta.Cerrada or D.EstadoCuenta.Anulada)
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

            // devuelve la entidad (no solo el Id)
            var detalle = comanda.AgregarItem(producto.Id, req.Cantidad, producto.Precio, producto.CostoEstandar, req.Observacion);            

            //await _uow.SaveChangesAsync(ct);
            try
            {
                // fuerza INSERT (Added) aunque EF “quiera” UPDATE
                await _comandas.AddDetalleAsync(detalle, ct);
                await _uow.SaveChangesAsync(ct);
                return detalle.Id;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var names = ex.Entries.Select(e => e.Metadata.Name).ToArray();
                throw new InvalidOperationException(
                    $"Conflicto de concurrencia al guardar. Entidades: {string.Join(", ", names)}", ex);
            }

            // El último detalle creado (en EF se guardará con Id)
            //var last = comanda.Detalles.Last();
            //return last.Id;            
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
                cuenta.Tipo.ToShared(),
                cuenta.Estado.ToShared(),
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
