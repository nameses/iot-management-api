using iot_management_api.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace iot_management_api.Helper
{
    public class JwtGenerator
    {
        private readonly IOptions<JwtConfig> _conf;

        public JwtGenerator(IOptions<JwtConfig> conf)
        {
            _conf=conf;
        }
        public string GenerateToken(int userId)
        {
            var secret = Encoding.ASCII.GetBytes(_conf.Value.Key!);
            var securityKey = new SymmetricSecurityKey(secret);
            var issuer = _conf.Value.Issuer;
            var audience = _conf.Value.Audience;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", userId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
