using RestaurantSystem.Shared.Contracts;
using RestaurantSystem.Shared.Enums;
using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantSystem.Client.Services
{
    public sealed class CocinaApi
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;
        public CocinaApi(HttpClient http, JsonSerializerOptions json)
        {
            _http = http;
            _json = json;
        }

        public Task<List<KdsCardDto>> GetKdsAsync(EstadoCocinaItem? estado, CancellationToken ct = default)
        {
            var url = estado is null ? "api/cocina/kds" : $"api/cocina/kds?estado={estado}";
            return _http.GetFromJsonAsync<List<KdsCardDto>>(url, _json, ct)!;
        }

        public async Task CambiarEstadoItemAsync(Guid comandaDetalleId, EstadoCocinaItem estado, CancellationToken ct = default)
        {
            var resp = await _http.PostAsync($"api/cocina/items/{comandaDetalleId}/estado?estado={estado}", null, ct);
            resp.EnsureSuccessStatusCode();
        }
    }
}
