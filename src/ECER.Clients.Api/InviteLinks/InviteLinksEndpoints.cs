using ECER.Managers.Admin.Contract.InviteLinks;
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

      return TypedResults.Ok(new GenerateInviteLinkResponse(referenceLinkResponse.portalInvitation, referenceLinkResponse.verificationLink));
    })
.WithOpenApi("Creates a new reference link", string.Empty, "references_post")
.RequireAuthorization()
.DisableAntiforgery()
.WithParameterValidation();
  }
}

public record GenerateInviteLinkRequest(Guid portalInvitation, InviteType inviteType, int validDays);
public record GenerateInviteLinkResponse(Guid portalInvitation, string inviteLink);
