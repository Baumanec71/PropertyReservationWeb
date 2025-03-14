using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PropertyReservationWeb.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PropertyReservationWeb.Domain.Helpers
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _configuration;
        public JwtProvider(IConfiguration configuration)
        {
             _configuration = configuration;
        }
        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var secretKey = _configuration["JwtOptions:Key"];
            var issuer = _configuration["JwtOptions:Issuer"];
            var audience = _configuration["JwtOptions:Audience"];  

            if (!double.TryParse(_configuration["JwtOptions:ExpiresHours"], out double expiresHours))
            {
                expiresHours = 12;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiresHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public interface IJwtProvider
    {
        string GenerateJwtToken(User user);
    }
}
