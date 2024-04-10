using ECER.Managers.Admin.Contract.PortalInvitations;
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
      var portalInvitationQueryResults = await messageBus.Send(new InviteLinkQuery(token), ct);
      var portalInvitation = portalInvitationQueryResults.Items.SingleOrDefault();
      var registrant = portalInvitation!.Registrant!.Profile;

      return TypedResults.Ok(new ReferenceQueryResult(portalInvitation.firstName, portalInvitation.lastName, portalInvitation.emailAddress, registrant.FirstName!, registrant.LastName!));
    }).WithOpenApi("Handles references queries", string.Empty, "references_get").WithParameterValidation();
  }
}

public record ReferenceQueryResult(string referenceFirstName, string referenceLastName, string referenceEmail, string applicantFirstName, string applicantLastName);
