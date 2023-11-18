using iot_management_api.Configuration;
using iot_management_api.Entities.common;
using iot_management_api.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Iot_xunit_tests.Tests.Jwt
{
    public class JwtGeneratorTests
    {
        [Fact]
        public void GenerateToken_ValidTokenGenerated()
        {
            // Arrange
            var jwtConfigMock = new Mock<IOptions<JwtConfig>>();
            jwtConfigMock.Setup(config => config.Value).Returns(new JwtConfig
            {
                Key = "FXPHmFpoXzGZ9NegXlFqoecQz9niMXMc",
                Issuer = "react-iot-management",
                Audience = "webapi-iot-management-api"
            });

            var jwtGenerator = new JwtGenerator(jwtConfigMock.Object);
            var userId = 123;
            var email = "test@example.com";
            var userRole = UserRole.Student;

            // Act
            var token = jwtGenerator.GenerateToken(userId, email, userRole);

            // Assert
            Assert.NotNull(token);
            Assert.True(IsTokenValid(token, jwtConfigMock.Object.Value));
        }

        private bool IsTokenValid(string token, JwtConfig jwtConfig)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtConfig.Key);
            var securityKey = new SymmetricSecurityKey(key);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = securityKey
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
