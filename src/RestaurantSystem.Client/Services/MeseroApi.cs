using RestaurantSystem.Shared.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantSystem.Client.Services
{
    public sealed class MeseroApi
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;
        public MeseroApi(HttpClient http, JsonSerializerOptions json)
        {
            _http = http;
            _json = json;
        }

        public async Task<List<MesaResumenDto>> GetMesasAsync(CancellationToken ct = default)
            => await _http.GetFromJsonAsync<List<MesaResumenDto>>("api/mesero/mesas", _json, ct) ?? new();

        public async Task<List<CuentasActivasParaLlevarDto>> GetCuentasActivasParaLlevarAsync(CancellationToken ct = default)
            => await _http.GetFromJsonAsync<List<CuentasActivasParaLlevarDto>>("api/mesero/activas-para-llevar", _json, ct) ?? new();

        public async Task<Guid> AbrirCuentaAsync(AbrirCuentaRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/mesero/cuentas", req, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<Guid>(_json, ct);
        }

        public async Task<CuentaDetalleDto> GetCuentaDetalleAsync(Guid cuentaId, CancellationToken ct = default)
            => (await _http.GetFromJsonAsync<CuentaDetalleDto>($"api/mesero/cuentas/{cuentaId}", _json, ct))!;

        public async Task<CrearComandaResponse> CrearComandaAsync(Guid cuentaId, CancellationToken ct = default)
        {
            var resp = await _http.PostAsync($"api/mesero/cuentas/{cuentaId}/comandas", null, ct);
            resp.EnsureSuccessStatusCode();
            return (await resp.Content.ReadFromJsonAsync<CrearComandaResponse>(_json, ct))!;
        }

        public async Task SolicitarCuentaAsync(Guid cuentaId, CancellationToken ct = default)
        {
            var resp = await _http.PostAsync($"api/mesero/cuentas/{cuentaId}/solicitar", null, ct);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<Guid> AgregarItemAsync(Guid comandaId, AgregarItemRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync($"api/mesero/comandas/{comandaId}/items", req, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<Guid>(_json, ct);
        }

        public async Task EnviarACocinaAsync(Guid comandaId, bool imprimir, CancellationToken ct = default)
        {
            var req = new EnviarACocinaRequest(ImprimirComanda: imprimir);
            var resp = await _http.PostAsJsonAsync($"api/mesero/comandas/{comandaId}/enviar", req, ct);
            resp.EnsureSuccessStatusCode();
        }
    }
}
