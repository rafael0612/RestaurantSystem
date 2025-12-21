using RestaurantSystem.Application.Common;
using RestaurantSystem.Shared.Contracts;
using RestaurantSystem.Shared.Enums;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface IKdsQueryRepository
    {
        Task<List<KdsCardDto>> GetKdsAsync(EstadoCocinaItem? estadoFiltro, CancellationToken ct);
    }
}
