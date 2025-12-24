using RestaurantSystem.Shared.Contracts;
using RestaurantSystem.Shared.Enums;
using System.Net.Http.Json;
using System.Text;
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

        public async Task<List<KdsCardDto>> GetKdsAsync(EstadoCocinaItem? estado, CancellationToken ct = default)
        {
            var url = estado is null 
                ? "api/cocina/kds" 
                : $"api/cocina/kds?estado={estado.Value}";
            return await _http.GetFromJsonAsync<List<KdsCardDto>>(url, _json, ct) ?? new();
        }

        public async Task CambiarEstadoItemAsync(Guid comandaDetalleId, EstadoCocinaItem nuevoEstado, CancellationToken ct = default)
        {
            // PATCH con body = enum (como espera tu controller)
            //var content = new StringContent(JsonSerializer.Serialize(nuevoEstado, _json), Encoding.UTF8, "application/json");
            //var resp = await _http.PostAsync($"api/cocina/items/{comandaDetalleId}/estado", content, ct);
            //resp.EnsureSuccessStatusCode();

            // PATCH + body JSON
            var req = new HttpRequestMessage(HttpMethod.Patch, $"api/cocina/items/{comandaDetalleId}/estado")
            {
                // Recomendado: enviar como número para evitar problemas si tu API no usa JsonStringEnumConverter
                Content = JsonContent.Create((int)nuevoEstado, options: _json)
                // Alternativa si ya tienes enums como string en API: JsonContent.Create(nuevoEstado, options: _json)
            };

            var resp = await _http.SendAsync(req, ct);
            resp.EnsureSuccessStatusCode();
        }
    }
}
