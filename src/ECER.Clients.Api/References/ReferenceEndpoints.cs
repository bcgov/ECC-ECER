using ECER.Managers.Admin.Contract.References;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.Api.References;

public class ReferenceEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/references", async Task<Results<Ok<PortalInvitationToLinkResponse>, BadRequest<string>>> (
      PortalInvitationToLinkRequest request,
      HttpContext httpContext,
      IMediator messageBus,
      CancellationToken ct) =>
    {
      var referenceLinkResponse = await messageBus.Send(new GenerateReferenceLinkCommand(request.portalInvitation, request.referenceType), ct);
      return TypedResults.Ok(referenceLinkResponse);
    })
.WithOpenApi("Creates a new reference link", string.Empty, "references_post")
.RequireAuthorization()
.DisableAntiforgery()
.WithParameterValidation();
  }
}
