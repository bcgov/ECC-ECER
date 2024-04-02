using ECER.Managers.Admin.Contract.References;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.Api.References;

public class ReferenceEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/references", async Task<Results<Ok<GenerateVerificationLinkResponse>, BadRequest<string>>> (
      PortalInvitationToLinkRequest request,
      HttpContext httpContext,
      IMediator messageBus,
      IConfiguration configuration,
      CancellationToken ct) =>
    {
      var referenceLinkResponse = await messageBus.Send(new GenerateReferenceLinkCommand(request.portalInvitation, request.referenceType), ct);
      string baseUrl = configuration["PortalApp:BaseUrl"]!;
      string referenceVerificationRoute = configuration["PortalApp:ReferenceVerificationRoute"]!;
      string verificationLink = $"{baseUrl}/{referenceVerificationRoute}/{referenceLinkResponse.encryptedVerificationToken}";

      return TypedResults.Ok(new GenerateVerificationLinkResponse(referenceLinkResponse.portalInvitation, verificationLink));
    })
.WithOpenApi("Creates a new reference link", string.Empty, "references_post")
.RequireAuthorization()
.DisableAntiforgery()
.WithParameterValidation();
  }
}

public record GenerateVerificationLinkResponse(Guid portalInvitation, string verificationLink);
