using System.Security.Claims;
using System.Text.Json;

namespace RestaurantSystem.Client.Auth
{
    public static class JwtParser
    {
        public static ClaimsPrincipal ToClaimsPrincipal(string jwt)
        {
            var identity = new ClaimsIdentity(ParseClaims(jwt), authenticationType: "jwt");
            return new ClaimsPrincipal(identity);
        }

        private static IEnumerable<Claim> ParseClaims(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3) yield break;

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);

            var kv = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            if (kv is null) yield break;

            foreach (var (k, v) in kv)
            {
                // role puede venir como string o array según emisor.
                if (k is "role" or ClaimTypes.Role)
                {
                    if (v is JsonElement je)
                    {
                        if (je.ValueKind == JsonValueKind.String)
                        {
                            yield return new Claim(ClaimTypes.Role, je.GetString()!);
                            continue;
                        }
                        if (je.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var r in je.EnumerateArray())
                                yield return new Claim(ClaimTypes.Role, r.GetString()!);
                            continue;
                        }
                    }
                }

                yield return new Claim(k, v?.ToString() ?? "");
            }
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
