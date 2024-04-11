using ECER.Managers.Admin.Contract.PortalInvitations;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.Api.References;

public class InviteLinksEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/invitelinks", async Task<Results<Ok<GenerateInviteLinkResponse>, BadRequest<string>>> (
      GenerateInviteLinkRequest request,
      HttpContext httpContext,
      IMediator messageBus,
      CancellationToken ct) =>
    {
      var referenceLinkResponse = await messageBus.Send(new GenerateInviteLinkCommand(request.portalInvitation, request.inviteType, request.validDays), ct);

      return TypedResults.Ok(new GenerateInviteLinkResponse(referenceLinkResponse.PortalInvitation, referenceLinkResponse.VerificationLink));
    })
.WithOpenApi("Creates a new reference link", string.Empty, "references_post")
.DisableAntiforgery()
.WithParameterValidation()
.RequireAuthorization();
  }
}

public record GenerateInviteLinkRequest(Guid portalInvitation, InviteType inviteType, int validDays);
public record GenerateInviteLinkResponse(Guid portalInvitation, string inviteLink);
