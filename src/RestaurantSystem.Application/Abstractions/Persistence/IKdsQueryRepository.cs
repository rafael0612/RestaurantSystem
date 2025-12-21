using RestaurantSystem.Application.Dtos;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Abstractions.Persistence
{
    public interface IKdsQueryRepository
    {
        Task<List<KdsCardDto>> GetKdsAsync(EstadoCocinaItem? estadoFiltro, CancellationToken ct);
    }
}
