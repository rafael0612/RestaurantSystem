using RestaurantSystem.Shared.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantSystem.Client.Services
{
    public sealed class UsuariosApi
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;
        public UsuariosApi(HttpClient http, JsonSerializerOptions json) 
        { 
            _http = http;
            _json = json;
        }

        public async Task<Guid> CrearAsync(CrearUsuarioRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/usuarios", req, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<Guid>(cancellationToken: ct);
        }

        public async Task<List<UsuarioDto>> ListarAsync(CancellationToken ct = default)
            => await _http.GetFromJsonAsync<List<UsuarioDto>>("api/usuarios", _json, ct) ?? new();

        public async Task CambiarRolAsync(Guid id, CambiarRolRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PatchAsJsonAsync($"api/usuarios/{id}/rol", req, ct);
            resp.EnsureSuccessStatusCode();
        }

        public async Task CambiarEstadoAsync(Guid id, CambiarEstadoRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PatchAsJsonAsync($"api/usuarios/{id}/estado", req, ct);
            resp.EnsureSuccessStatusCode();
        }

        public async Task ResetPasswordAsync(Guid id, ResetPasswordRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PutAsJsonAsync($"api/usuarios/{id}/password", req, ct);
            resp.EnsureSuccessStatusCode();
        }
    }
}
