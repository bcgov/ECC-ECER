using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ECER.Utilities.Hosting;

public static class RouteBuildingExtensions
{
  public static RouteHandlerBuilder WithOpenApi(this RouteHandlerBuilder builder, string summary, string description, string operationId)
  {
    return builder
      .WithName(operationId)
      .WithSummary(summary)
      .WithDescription(description);
  }
}
