using iot_management_api.Configuration;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Iot_xunit_tests.Tests.Helper
{
    public class JwtValidatorTests
    {
        [Fact]
        public void Validate_ValidToken_ReturnsUserId()
        {
            // Arrange
            var jwtConfigMock = new Mock<IOptions<JwtConfig>>();
            jwtConfigMock.Setup(config => config.Value).Returns(new JwtConfig
            {
                Key = "FXPHmFpoXzGZ9NegXlFqoecQz9niMXMc",
                Issuer = "react-iot-management",
                Audience = "webapi-iot-management-api"
            });

            var loggerMock = new Mock<ILogger<JwtValidator>>();
            var validator = new JwtValidator(jwtConfigMock.Object, loggerMock.Object);

            var token = GenerateValidToken();

            // Act
            var userId = validator.Validate(token);

            // Assert
            Assert.NotNull(userId);
            Assert.Equal(123, userId);
        }

        [Fact]
        public void Validate_ExpiredToken_ReturnsNull()
        {
            // Arrange
            var jwtConfigMock = new Mock<IOptions<JwtConfig>>();
            jwtConfigMock.Setup(config => config.Value).Returns(new JwtConfig
            {
                Key = "FXPHmFpoXzGZ9NegXlFqoecQz9niMXMc",
                Issuer = "react-iot-management",
                Audience = "webapi-iot-management-api"
            });

            var loggerMock = new Mock<ILogger<JwtValidator>>();
            var validator = new JwtValidator(jwtConfigMock.Object, loggerMock.Object);

            var expiredToken = GenerateExpiredToken();

            // Act
            var userId = validator.Validate(expiredToken);

            // Assert
            Assert.Null(userId);
        }

        [Fact]
        public void Validate_InvalidSignature_ReturnsNull()
        {
            // Arrange
            var jwtConfigMock = new Mock<IOptions<JwtConfig>>();
            jwtConfigMock.Setup(config => config.Value).Returns(new JwtConfig
            {
                Key = "FXPHmFpoXzGZ9NegXlFqoecQz9niMXMc",
                Issuer = "react-iot-management",
                Audience = "webapi-iot-management-api"
            });

            var loggerMock = new Mock<ILogger<JwtValidator>>();
            var validator = new JwtValidator(jwtConfigMock.Object, loggerMock.Object);

            var invalidSignatureToken = GenerateTokenWithInvalidSignature();

            // Act
            var userId = validator.Validate(invalidSignatureToken);

            // Assert
            Assert.Null(userId);
        }



        private int userId = 123;
        private string email = "test@example.com";
        private UserRole userRole = UserRole.Student;

        private string GenerateValidToken()
        {
            var secret = Encoding.ASCII.GetBytes("FXPHmFpoXzGZ9NegXlFqoecQz9niMXMc");
            var securityKey = new SymmetricSecurityKey(secret);
            var issuer = "react-iot-management";
            var audience = "webapi-iot-management-api";


            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", userId.ToString()),
                    new Claim("email", email),
                    new Claim(ClaimTypes.Role, userRole.ToString()),
                    new Claim("role", userRole.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateExpiredToken()
        {
            var validToken = GenerateValidToken();
            var tokenHandler = new JwtSecurityTokenHandler();
            var expiredToken = tokenHandler.ReadToken(validToken) as JwtSecurityToken;

            var newExpirationTime = DateTime.UtcNow.AddMinutes(-1);
            var newNotBeforeTime = newExpirationTime.AddMinutes(-1);

            var newTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", userId.ToString()),
                    new Claim("email", email),
                    new Claim(ClaimTypes.Role, userRole.ToString()),
                    new Claim("role", userRole.ToString())
                }),
                NotBefore = newNotBeforeTime,
                Expires = newExpirationTime,
                Issuer = expiredToken.Issuer,
                Audience = expiredToken.Audiences.First(),
                SigningCredentials = expiredToken.SigningKey != null
                    ? new SigningCredentials(expiredToken.SigningKey, SecurityAlgorithms.HmacSha256Signature)
                    : null
            };

            var newToken = tokenHandler.CreateToken(newTokenDescriptor);
            return tokenHandler.WriteToken(newToken);
        }

        private string GenerateTokenWithInvalidSignature()
        {
            var validToken = GenerateValidToken();
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenWithInvalidSignature = tokenHandler.ReadToken(validToken) as JwtSecurityToken;

            var newSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("AbPHmFpoXzGZ9NdfXlFqoecQz9niMXMc"));
            var newTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", userId.ToString()),
                    new Claim("email", email),
                    new Claim(ClaimTypes.Role, userRole.ToString()),
                    new Claim("role", userRole.ToString())
                }),
                Expires = tokenWithInvalidSignature.ValidTo,
                Issuer = tokenWithInvalidSignature.Issuer,
                Audience = tokenWithInvalidSignature.Audiences.First(),
                SigningCredentials = new SigningCredentials(newSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var newToken = tokenHandler.CreateToken(newTokenDescriptor);
            return tokenHandler.WriteToken(newToken);
        }

    }

}
