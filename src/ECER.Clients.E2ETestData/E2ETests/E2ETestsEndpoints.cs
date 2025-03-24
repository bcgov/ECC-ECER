
using Microsoft.AspNetCore.Http.HttpResults;
using ECER.Utilities.Hosting;
using MediatR;
using ECER.Managers.E2ETest.Contract.Applications;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.E2ETestData.E2ETests;

public class E2ETestsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {

    endpointRouteBuilder.MapDelete("/api/E2ETests/applications/delete/{application_id}", async Task<Results<Ok<string>, BadRequest<ProblemDetails>, NotFound>> (string application_id, HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
    {

      bool IdIsNotGuid = !Guid.TryParse(application_id, out _);

      if (IdIsNotGuid)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Title = "Application ID is not valid" });
      }

      var result = await messageBus.Send(new E2ETestsDeleteApplicationCommand(application_id), ct);
      return TypedResults.Ok(result);
    })
    .WithOpenApi("Handles Delete Application Command for E2E Tests", string.Empty, "E2ETests_delete_application")
    .RequireAuthorization()
    .DisableAntiforgery()
    .WithParameterValidation();
  }
}
