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

        public async Task<CajaSesionDto?> GetSesionAsync(CancellationToken ct = default)
        {
            var resp = await _http.GetAsync("api/caja/sesion", ct);
            if (resp.StatusCode == System.Net.HttpStatusCode.NoContent) return null;

            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CajaSesionDto>(_json, ct);
        }

        public async Task<CajaSesionDto> AbrirCajaAsync(AbrirCajaRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/caja/abrir", req, ct);
            resp.EnsureSuccessStatusCode();
            return (await resp.Content.ReadFromJsonAsync<CajaSesionDto>(cancellationToken: ct))!;
        }

        public async Task RegistrarEgresoAsync(RegistrarEgresoRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/caja/egresos", req, _json, ct);
            resp.EnsureSuccessStatusCode();
        }

        public Task<List<CuentaPorCobrarDto>> ListarCuentasPorCobrarAsync(CancellationToken ct = default)
            => _http.GetFromJsonAsync<List<CuentaPorCobrarDto>>("api/caja/cuentas", _json, ct) ?? Task.FromResult(new List<CuentaPorCobrarDto>());

        public async Task<CuentaCobroDto> GetCuentaCobroAsync(Guid cuentaId, CancellationToken ct = default)
            => (await _http.GetFromJsonAsync<CuentaCobroDto>($"api/caja/cuentas/{cuentaId}/cobro", _json, ct))!;


        public async Task<RegistrarPagoResponse> RegistrarPagoAsync(RegistrarPagoRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/caja/pagos", req, _json, ct);
            resp.EnsureSuccessStatusCode();
            return (await resp.Content.ReadFromJsonAsync<RegistrarPagoResponse>(_json, ct))!;
        }

        public async Task<CerrarCajaResponse> CerrarCajaAsync(CerrarCajaRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/caja/cerrar", req, _json, ct);
            resp.EnsureSuccessStatusCode();
            return (await resp.Content.ReadFromJsonAsync<CerrarCajaResponse>(_json, ct))!;
        }

        public async Task<ReporteDiarioCajaDto> ReporteAsync(DateOnly? fecha = null, CancellationToken ct = default)
        {
            var q = fecha is null ? "" : $"?fecha={fecha:yyyy-MM-dd}";
            return (await _http.GetFromJsonAsync<ReporteDiarioCajaDto>($"api/caja/reporte{q}", _json, ct))!;
        }
    }
}
