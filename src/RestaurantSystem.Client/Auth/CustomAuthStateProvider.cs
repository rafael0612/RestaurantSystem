using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace RestaurantSystem.Client.Auth
{
    public sealed class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private static readonly ClaimsPrincipal Anonymous =
            new(new ClaimsIdentity());

        private readonly ITokenStorage _tokens;

        public CustomAuthStateProvider(ITokenStorage tokens) => _tokens = tokens;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var (token, exp) = await _tokens.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return new AuthenticationState(Anonymous);

            // Expiración simple (evita tener sesión inválida)
            if (exp.HasValue && exp.Value <= DateTime.UtcNow)
            {
                await _tokens.ClearAsync();
                return new AuthenticationState(Anonymous);
            }

            var user = JwtParser.ToClaimsPrincipal(token);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string jwt)
        {
            var user = JwtParser.ToClaimsPrincipal(jwt);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
        }
    }
}
