using ECER.Infrastructure.Common.Validators;
using ECER.Managers.Registry.Contract.Reconsiderations;
using ECER.Utilities.Hosting;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.Security;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ECER.Clients.RegistryPortal.Server.Reconsiderations;

public class ReconsiderationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/reconsiderations", async Task<Results<Ok<IEnumerable<Reconsideration>>, NotFound>> ([FromQuery(Name = "ByStatusCodes[]")]ReconsiderationStatusCode[]? ByStatusCodes, [FromQuery] string? ById, [FromQuery] string? ByApplicationId, HttpContext ctx, IMediator messagebus, IReconsiderationsMapper reconsiderationsMapper, CancellationToken ct) =>
    {
      var userContext = ctx.User.GetUserContext();

      var command = new ReconsiderationQueryCommand()
      {
        ByApplicantId = userContext!.UserId,
        ById = ById,
        ByApplicationId = ByApplicationId,
        ByStatusCodes = ByStatusCodes != null && ByStatusCodes.Length > 0 ? reconsiderationsMapper.MapReconsiderationStatusCodes(ByStatusCodes) : null,
      };

      var results = await messagebus.Send(command, ct);

      return TypedResults.Ok(reconsiderationsMapper.MapReconsiderationRequests(results.Items));
    }).WithOpenApi("Handles reconsiderations queries", string.Empty, "reconsiderations_get")
    .RequireAuthorization()
    .AddGuidValidationQueryParams(["ById", "ByApplicationId"], false)
    .WithParameterValidation();

    endpointRouteBuilder.MapPut("/api/reconsiderations/submit/{id}", async Task<Results<Ok<string>, BadRequest<ProblemDetails>>> ([FromBody] Reconsideration Reconsideration, [FromRoute] string id, HttpContext ctx, IMediator messagebus, IReconsiderationsMapper reconsiderationsMapper, CancellationToken ct) =>
    {
      if (Reconsideration.Id != id) return TypedResults.BadRequest(new ProblemDetails()
      {
        Detail = "resource id and payload id do not match"
      });

      var userContext = ctx.User.GetUserContext();

      var command = new ReconsiderationSubmitCommand(reconsiderationsMapper.MapReconsiderationRequest(Reconsideration), userContext!.UserId);

      var response = await messagebus.Send(command, ct);

      if (!response.IsSuccess)
      {
        switch (response.ErrorCode)
        {
          case ReconsiderationSubmitErrorCode.ReconsiderationNotFound:
            return TypedResults.BadRequest(new ProblemDetails()
            {
              Detail = "reconsideration not found"
            });

          case ReconsiderationSubmitErrorCode.ReconsiderationWrongStatus:
            return TypedResults.BadRequest(new ProblemDetails()
            {
              Detail = "reconsideration is in the wrong status"
            });
        }
      }

      return TypedResults.Ok(response.Id);
    }).WithOpenApi("Handles reconsiderations queries", string.Empty, "reconsiderations_submit_put")
      .RequireAuthorization()
      .AddGuidValidation("id")
      .WithParameterValidation();
  }
}

/// <summary>
/// Represents a reconsideration request submitted by an applicant.
/// </summary>
public record Reconsideration()
{
  [ValidGuid]
  public string? Id { get; set; }
  public string? ReconsiderationDetails { get; set; }
  public string? ExplanationAndEvidence { get; set; }
  public ReconsiderationStatusCode Status { get; set; }
  public IEnumerable<Applications.FileInfo> Files { get; set; } = Array.Empty<Applications.FileInfo>();
  public DateTime? ReconsiderationEndDate { get; set; }
  [ValidGuid]
  public string? ApplicationId { get; set; }
}

public enum ReconsiderationStatusCode
{
  Complete,
  InReview,
  New,
}
