using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MedicSoft.Api.Authentication
{
    /// <summary>
    /// Authentication handler that bypasses JWT authentication in development environment
    /// </summary>
    public class DevBypassAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public DevBypassAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Create a mock user identity for development
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "dev-user"),
                new Claim("tenant_id", "default-tenant"),
                new Claim("user_id", "dev-user-id")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
