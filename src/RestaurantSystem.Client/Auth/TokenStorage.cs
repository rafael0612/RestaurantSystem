using Blazored.LocalStorage;
namespace RestaurantSystem.Client.Auth
{
    public interface ITokenStorage
    {
        Task SetTokenAsync(string token, DateTime expiresAtUtc);
        Task<(string? token, DateTime? expiresAtUtc)> GetTokenAsync();
        Task ClearAsync();
    }
    public sealed class TokenStorage : ITokenStorage
    {
        private const string TokenKey = "auth.token";
        private const string ExpKey = "auth.expiresAtUtc";

        private readonly ILocalStorageService _ls;
        public TokenStorage(ILocalStorageService ls) => _ls = ls;

        public async Task SetTokenAsync(string token, DateTime expiresAtUtc)
        {
            await _ls.SetItemAsync(TokenKey, token);
            await _ls.SetItemAsync(ExpKey, expiresAtUtc);
        }

        public async Task<(string? token, DateTime? expiresAtUtc)> GetTokenAsync()
        {
            var token = await _ls.GetItemAsync<string>(TokenKey);
            var exp = await _ls.GetItemAsync<DateTime?>(ExpKey);
            return (token, exp);
        }

        public async Task ClearAsync()
        {
            await _ls.RemoveItemAsync(TokenKey);
            await _ls.RemoveItemAsync(ExpKey);
        }
    }
}
