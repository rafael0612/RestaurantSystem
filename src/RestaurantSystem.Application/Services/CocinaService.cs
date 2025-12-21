using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Application.Common;
using RestaurantSystem.Shared.Contracts;
using D = RestaurantSystem.Domain.Enums;
using S = RestaurantSystem.Shared.Enums;

namespace RestaurantSystem.Application.Services
{
    public interface ICocinaService
    {
        Task<List<KdsCardDto>> GetKdsAsync(S.EstadoCocinaItem? estadoFiltro, CancellationToken ct);
        Task CambiarEstadoItemAsync(Guid comandaDetalleId, S.EstadoCocinaItem nuevoEstado, CancellationToken ct);
    }

    public class CocinaService : ICocinaService
    {
        private readonly IComandaRepository _comandas;
        private readonly IUnitOfWork _uow;
        private readonly IKdsQueryRepository _kds;

        public CocinaService(IComandaRepository comandas, IKdsQueryRepository kds, IUnitOfWork uow)
        {
            _comandas = comandas;
            _kds = kds;
            _uow = uow;
        }

        public Task<List<KdsCardDto>> GetKdsAsync(S.EstadoCocinaItem? estadoFiltro, CancellationToken ct)
            => _kds.GetKdsAsync(estadoFiltro, ct);

        public async Task CambiarEstadoItemAsync(Guid comandaDetalleId, S.EstadoCocinaItem nuevoEstado, CancellationToken ct)
        {
            var item = await _comandas.GetDetalleByIdAsync(comandaDetalleId, ct)
                       ?? throw new KeyNotFoundException("Item no existe.");

            item.CambiarEstadoCocina(nuevoEstado.ToDomain());

            // Actualiza estado de la comanda según items (usaremos método dominio)
            var comanda = await _comandas.GetByIdAsync(item.ComandaId, includeDetails: true, ct)
                         ?? throw new KeyNotFoundException("Comanda no existe.");

            comanda.MarcarListoSiCorresponde();

            await _uow.SaveChangesAsync(ct);
        }
    }
}
