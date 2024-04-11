using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.RegistryPortal.Server.References;

public class PortalInvitationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/PortalInvitations/{token?}", async Task<Results<Ok<ReferenceQueryResult>, BadRequest<string>>> (string? token, IMediator messageBus, HttpContext httpContext, CancellationToken ct) =>
    {
      if (token == null) return TypedResults.BadRequest("Token is required");
      var result = await messageBus.Send(new PortalInvitationVerificationQuery(token), ct);

      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(result.ErrorMessage);
      }
      return TypedResults.Ok(new ReferenceQueryResult(result.Invitation!));
    }).WithOpenApi("Handles references queries", string.Empty, "references_get").WithParameterValidation();
  }
}

public record ReferenceQueryResult(PortalInvitation PortalInvitation);
