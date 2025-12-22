using System.Net.Http.Headers;

namespace RestaurantSystem.Client.Auth
{
    public sealed class BearerTokenHandler : DelegatingHandler
    {
        private readonly ITokenStorage _tokens;
        public BearerTokenHandler(ITokenStorage tokens) => _tokens = tokens;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var (token, exp) = await _tokens.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token) && (!exp.HasValue || exp.Value > DateTime.UtcNow))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
