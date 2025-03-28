using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ECER.Clients.E2ETestData.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string ApiKeyHeaderName = "X-API-KEY";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            await Task.CompletedTask;

            // Ensure the API key header exists.
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.Fail("Missing API Key header.");
            }

            string providedApiKey = apiKeyHeaderValues.FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.Fail("Invalid API Key header.");
            }


            var configuration = Context.RequestServices.GetRequiredService<IConfiguration>();
            string? validApiKey = configuration["ApiKey"];

            if (string.IsNullOrWhiteSpace(validApiKey))
            {
                // Optionally log this as a configuration error.
                return AuthenticateResult.Fail("API Key is not configured.");
            }

            if (!string.Equals(providedApiKey, validApiKey, StringComparison.Ordinal))
            {
                return AuthenticateResult.Fail("Invalid API Key.");
            }

            // Create the authenticated principal.
            var claims = new[] { new Claim(ClaimTypes.Name, "APIKeyUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
