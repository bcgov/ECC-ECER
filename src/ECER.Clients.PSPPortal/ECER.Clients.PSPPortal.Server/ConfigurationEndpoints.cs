using ECER.Utilities.Hosting;
using MediatR;

namespace ECER.Clients.PSPPortal.Server;

public class ConfigurationEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/configuration", async (HttpContext ctx, IMediator messageBus, CancellationToken ct) =>
    {
      await Task.CompletedTask;
      var configuration = ctx.RequestServices.GetRequiredService<IConfiguration>();
      var appConfig = configuration.Get<ApplicationConfiguration>()!;
      return TypedResults.Ok(appConfig);
    }).WithOpenApi("Returns the UI initial configuration", string.Empty, "configuration_get")
      .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));
  }
}

#pragma warning disable CA2227 // Collection properties should be read only

public record ApplicationConfiguration
{
  public Dictionary<string, OidcAuthenticationSettings> ClientAuthenticationMethods { get; set; } = [];
}

#pragma warning restore CA2227 // Collection properties should be read only
public record OidcAuthenticationSettings
{
  public string Authority { get; set; } = null!;
  public string ClientId { get; set; } = null!;
  public string Scope { get; set; } = null!;
  public string? Idp { get; set; }
}
