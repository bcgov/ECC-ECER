using System;
using ECER.Utilities.Hosting;

namespace ECER.Clients.Api;

public class VersionEndpoint : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/version", async (HttpContext ctx) =>
        {
          await Task.CompletedTask;
          var version = VersionProvider.GetVersion();
          return TypedResults.Ok(version);
        }).WithOpenApi("Returns the api version information", string.Empty, "version_get");
  }
}
