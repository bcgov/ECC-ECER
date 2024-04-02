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
      PortalInvitationToLinkRequest request,
      HttpContext httpContext,
      IMediator messageBus,
      IConfiguration configuration,
      CancellationToken ct) =>
    {
      var referenceLinkResponse = await messageBus.Send(new GenerateInviteLinkCommand(request.portalInvitation, request.inviteType), ct);
      string baseUrl = configuration["PortalApp:BaseUrl"]!;
      string referenceVerificationRoute = configuration["PortalApp:ReferenceVerificationRoute"]!;
      string verificationLink = $"{baseUrl}/{referenceVerificationRoute}/{referenceLinkResponse.encryptedVerificationToken}";

      return TypedResults.Ok(new GenerateInviteLinkResponse(referenceLinkResponse.portalInvitation, verificationLink));
    })
.WithOpenApi("Creates a new reference link", string.Empty, "references_post")
.RequireAuthorization()
.DisableAntiforgery()
.WithParameterValidation();
  }
}

public record GenerateInviteLinkResponse(Guid portalInvitation, string inviteLink);
