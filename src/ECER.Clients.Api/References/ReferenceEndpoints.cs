﻿using ECER.Managers.Admin.Contract.References;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine;

namespace ECER.Clients.Api.References;

public class ReferenceEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/references", async Task<Results<Ok<GenerateReferenceLinkResponse>, BadRequest<string>>> (
      GenerateReferenceLinkRequest request,
HttpContext httpContext,
IMessageBus messageBus,
CancellationToken ct) =>
    {
      var referenceLinkResponse = await messageBus.InvokeAsync<GenerateReferenceLinkResponse>(new ReferenceLinkQuery(request.portalInvitation, request.referenceType), ct);
      return TypedResults.Ok(referenceLinkResponse);
    })
.WithOpenApi("Creates a new reference link", string.Empty, "references_post")
.RequireAuthorization()
.DisableAntiforgery()
.WithParameterValidation();
  }
}
