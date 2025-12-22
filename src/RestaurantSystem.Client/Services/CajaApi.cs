using RestaurantSystem.Shared.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantSystem.Client.Services
{
    public sealed class CajaApi
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;
        public CajaApi(HttpClient http, JsonSerializerOptions json) 
        { 
            _http = http; 
            _json = json;
        }

        public async Task<Guid> AbrirCajaAsync(decimal montoApertura, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/caja/abrir", new { montoApertura }, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<Guid>(cancellationToken: ct);
        }

        public async Task<PagoResultDto> RegistrarPagoAsync(RegistrarPagoRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/caja/pagos", req, ct);
            resp.EnsureSuccessStatusCode();
            return (await resp.Content.ReadFromJsonAsync<PagoResultDto>(cancellationToken: ct))!;
        }

        public async Task CerrarCajaAsync(CerrarCajaRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/caja/cerrar", req, ct);
            resp.EnsureSuccessStatusCode();
        }

        public Task<ReporteDiarioDto> ReporteDiarioAsync(DateOnly fecha, CancellationToken ct = default)
            => _http.GetFromJsonAsync<ReporteDiarioDto>($"api/caja/reporte?fecha={fecha:yyyy-MM-dd}", _json, ct)!;
    }
}
