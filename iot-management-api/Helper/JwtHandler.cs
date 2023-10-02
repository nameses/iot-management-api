using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace iot_management_api.Helper
{
    public class JwtHandler : JwtBearerHandler
    {
        private readonly ILogger<JwtHandler> _logger;
        private readonly JwtValidator _jwtValidator;

        public JwtHandler(ILogger<JwtHandler> logger,
            JwtValidator jwtValidator,
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, loggerFactory, encoder, clock)
        {
            _logger=logger;
            _jwtValidator=jwtValidator;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? token;

            if (ShouldSkip(Context)) return AuthenticateResult.NoResult();

            if (!Context.Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return AuthenticateResult.Fail("Authorization header not found.");
            }

            var authorizationHeader = authorizationHeaderValues.FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Bearer token not found in Authorization header.");
            }

            token = authorizationHeader.Substring("Bearer ".Length).Trim();

            var userId = _jwtValidator.Validate(token);

            if (userId==null)
            {
                return AuthenticateResult.Fail("Token validation failed.");
            }
            //else
            //Context.Items["User"] = _userService.GetById(userId);

            var principal = GetClaims(token);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, "JwtBearer"));
        }

        private static bool ShouldSkip(HttpContext context)
        {
            var excludedPaths = new[]
            {
                "/api/auth/register",
                "/api/auth/login",
                "/api/validate"
            };
            var requestPath = context.Request.Path;
            return excludedPaths.Any(path => requestPath.StartsWithSegments(path));
        }

        private static ClaimsPrincipal GetClaims(string Token)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(Token) as JwtSecurityToken;

            var claimsIdentity = new ClaimsIdentity(token?.Claims, "Token");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }
    }
}
