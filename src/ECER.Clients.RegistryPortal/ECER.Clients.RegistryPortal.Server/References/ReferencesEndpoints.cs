using ECER.Managers.Admin.Contract.InviteLinks;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.RegistryPortal.Server.References;

public class ReferencesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/references/{token?}", async Task<Results<Ok<ReferenceQueryResult>, BadRequest<string>>> (string? token, IMediator messageBus, HttpContext httpContext, CancellationToken ct) =>
    {
      if (token == null) return TypedResults.BadRequest("Token is required");
      var verificationResult = await messageBus.Send<VerifyInviteTokenCommandResponse>(new VerifyInviteTokenCommand(token), ct);
      if (verificationResult == null || verificationResult.portalInvitation == Guid.Empty) return TypedResults.BadRequest("Invalid token");

      return TypedResults.Ok(new ReferenceQueryResult(string.Empty, string.Empty));
    }).WithOpenApi("Handles references queries", string.Empty, "references_get")
      .RequireAuthorization();
  }
}

public record ReferenceQueryResult(string id, string name);
