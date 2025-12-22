using Microsoft.AspNetCore.Components.Authorization;
using RestaurantSystem.Client.Auth;
using RestaurantSystem.Shared.Contracts;
using System.Net.Http.Json;

namespace RestaurantSystem.Client.Services
{
    public sealed class AuthApi
    {
        private readonly HttpClient _http;
        private readonly ITokenStorage _tokens;
        private readonly CustomAuthStateProvider _auth;

        public AuthApi(HttpClient http, ITokenStorage tokens, AuthenticationStateProvider auth)
        {
            _http = http;
            _tokens = tokens;
            _auth = (CustomAuthStateProvider)auth;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/login", req, ct);
            if (!resp.IsSuccessStatusCode)
                throw new UnauthorizedAccessException("Login inválido.");

            var data = (await resp.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct))!;
            await _tokens.SetTokenAsync(data.AccessToken, data.ExpiresAtUtc);
            _auth.NotifyUserAuthentication(data.AccessToken);
            return data;
        }

        public async Task LogoutAsync()
        {
            await _tokens.ClearAsync();
            _auth.NotifyUserLogout();
        }
    }
}
