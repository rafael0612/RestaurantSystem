using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Dtos;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services
{
    public interface ICocinaService
    {
        Task<List<KdsCardDto>> GetKdsAsync(EstadoCocinaItem? estadoFiltro, CancellationToken ct);
        Task CambiarEstadoItemAsync(Guid comandaDetalleId, EstadoCocinaItem nuevoEstado, CancellationToken ct);
    }

    public class CocinaService : ICocinaService
    {
        private readonly IComandaRepository _comandas;
        private readonly ICuentaRepository _cuentas;
        private readonly IUnitOfWork _uow;
        private readonly IKdsQueryRepository _kds;

        public CocinaService(IComandaRepository comandas, ICuentaRepository cuentas, IKdsQueryRepository kds, IUnitOfWork uow)
        {
            _comandas = comandas;
            _cuentas = cuentas;
            _kds = kds;
            _uow = uow;
        }

        public Task<List<KdsCardDto>> GetKdsAsync(EstadoCocinaItem? estadoFiltro, CancellationToken ct)
            => _kds.GetKdsAsync(estadoFiltro, ct);

        public async Task CambiarEstadoItemAsync(Guid comandaDetalleId, EstadoCocinaItem nuevoEstado, CancellationToken ct)
        {
            var item = await _comandas.GetDetalleByIdAsync(comandaDetalleId, ct)
                       ?? throw new KeyNotFoundException("Item no existe.");

            item.CambiarEstadoCocina(nuevoEstado);

            // Actualiza estado de la comanda según items (usaremos método dominio)
            var comanda = await _comandas.GetByIdAsync(item.ComandaId, includeDetails: true, ct)
                         ?? throw new KeyNotFoundException("Comanda no existe.");

            comanda.MarcarListoSiCorresponde();

            await _uow.SaveChangesAsync(ct);
        }
    }
}
