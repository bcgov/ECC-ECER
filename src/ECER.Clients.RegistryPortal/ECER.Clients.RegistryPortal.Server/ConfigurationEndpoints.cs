using ECER.Utilities.Hosting;

namespace ECER.Clients.RegistryPortal.Server;

public class ConfigurationEndpoints : IRegisterEndpoints
{
    public void Register(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/api/configuration", async (HttpContext ctx) =>
        {
            await Task.CompletedTask;
            var configuration = ctx.RequestServices.GetRequiredService<IConfiguration>();
            var appConfig = configuration.Get<ApplicationConfiguration>();
            return TypedResults.Ok(appConfig);
        }).WithOpenApi("Frontend Configuration", "Frontend Configuration endpoint", "configuration");
    }
}

#pragma warning disable CA2227 // Collection properties should be read only

public class ApplicationConfiguration
{
    public Dictionary<string, OidcAuthenticationSettings> AuthenticationMethods { get; set; } = [];
}

#pragma warning restore CA2227 // Collection properties should be read only

public record OidcAuthenticationSettings
{
    public string Authority { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string Scope { get; set; } = null!;
}
