using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestaurantSystem.Application.Abstractions.Security;
using RestaurantSystem.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantSystem.API.Security
{
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _opt;

        public JwtTokenGenerator(IOptions<JwtOptions> opt) => _opt = opt.Value;

        public (string token, DateTime expiresAtUtc) CreateToken(Usuario user)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_opt.ExpireMinutes);

            var claims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new("username", user.Username),
            new("role", user.Rol.ToString()),
            new(ClaimTypes.Role, user.Rol.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return (token, expires);
        }
    }
}
