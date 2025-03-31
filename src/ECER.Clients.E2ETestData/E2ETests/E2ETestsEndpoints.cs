
using Microsoft.AspNetCore.Http.HttpResults;
using ECER.Utilities.Hosting;
using MediatR;
using ECER.Managers.E2ETest.Contract.E2ETestsContacts;
using Microsoft.AspNetCore.Mvc;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Security;

namespace ECER.Clients.E2ETestData.E2ETests;

public class E2ETestsEndpoints : IRegisterEndpoints
{
    public void Register(IEndpointRouteBuilder endpointRouteBuilder)
    {

        endpointRouteBuilder.MapDelete("/api/E2ETests/user/reset", async Task<Results<Ok<string>, BadRequest<ProblemDetails>, NotFound>> (HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
        {

          if (!ctx.Request.Headers.TryGetValue("EXTERNAL-USER-ID", out var externalUserId))
          {
            return TypedResults.BadRequest(new ProblemDetails { Title = "Missing header: EXTERNAL-USER-ID" });
          }


          var userIdentity = new UserIdentity (externalUserId.ToString(), "bcsc"); // Convert StringValues to UserIdentity
          var profile = (await messageBus.Send(new SearchRegistrantQuery { ByUserIdentity = userIdentity }, ctx.RequestAborted)).Items.SingleOrDefault();
          if (profile == null) return TypedResults.NotFound();
          var contact_id = profile.UserId;

          var result = await messageBus.Send(new E2ETestsDeleteContactApplicationsCommand(contact_id), ct);
            return TypedResults.Ok(result);
        })
        .WithOpenApi("Handles Deletion of all Applications and certificates of a User for E2E Tests", string.Empty, "E2ETests_delete_contact_applications")
        .RequireAuthorization()
        .DisableAntiforgery()
        .WithParameterValidation();
    }
}
