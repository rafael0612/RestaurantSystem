using RestaurantSystem.Shared.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantSystem.Client.Services
{
    public sealed class ProductosApi
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;
        public ProductosApi(HttpClient http, JsonSerializerOptions json)
        {
            _http = http;
            _json = json;
        }

        public async Task<List<ProductoDto>> ListActivosAsync(CancellationToken ct = default)
            => await _http.GetFromJsonAsync<List<ProductoDto>>("api/productos/activos", _json, ct) ?? new();
    }
}
