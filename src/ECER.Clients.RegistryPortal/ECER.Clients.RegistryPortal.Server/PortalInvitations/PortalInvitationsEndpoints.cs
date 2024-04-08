using ECER.Managers.Admin.Contract.PortalInvitations;
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
      var verificationResult = await messageBus.Send(new VerifyInviteTokenCommand(token), ct);
      if (verificationResult == null || verificationResult.portalInvitation == Guid.Empty) return TypedResults.BadRequest("Invalid token");
      // send portal invitation code and get reference details

      var portalInvitationQueryResults = await messageBus.Send(new PortalInvitationQuery(verificationResult.portalInvitation), ct);
      if (!portalInvitationQueryResults.Items.Any())
      {
        return TypedResults.BadRequest("Invalid token");
      }
      var portalInvitation = portalInvitationQueryResults.Items.SingleOrDefault();
      var registrant = portalInvitation!.Registrant!.Profile;

      return TypedResults.Ok(new ReferenceQueryResult(portalInvitation.Id, registrant.FirstName, registrant.LastName));
    }).WithOpenApi("Handles references queries", string.Empty, "references_get")
      .RequireAuthorization();
  }
}

public record ReferenceQueryResult(string? referenceId, string applicantFirstName, string applicantLastName);
