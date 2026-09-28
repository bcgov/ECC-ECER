using ECER.Infrastructure.Common.Validators;
using ECER.Managers.Registry.Contract.InvestigationReconsiderations;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.RegistryPortal.Server.InvestigationReconsiderations;

public class InvestigationReconsiderationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/investigation/reconsiderations", async Task<Results<Ok<IEnumerable<InvestigationReconsideration>>, NotFound>> ([FromQuery(Name = "ByStatusCodes[]")] InvestigationReconsiderationStatusCode[]? ByStatusCodes, [FromQuery] string? ById, [FromQuery] string? ByApplicationId, HttpContext ctx, IMediator messagebus, IInvestigationReconsiderationsMapper investigationReconsiderationsMapper, CancellationToken ct) =>
    {
      var userContext = ctx.User.GetUserContext();

      var command = new InvestigationReconsiderationQueryCommand()
      {
        ByApplicantId = userContext!.UserId,
        ById = ById,
        ByStatusCodes = ByStatusCodes != null && ByStatusCodes.Length > 0 ? investigationReconsiderationsMapper.MapInvestigationReconsiderationStatusCodes(ByStatusCodes) : null,
      };

      var results = await messagebus.Send(command, ct);

      return TypedResults.Ok(investigationReconsiderationsMapper.MapInvestigationReconsiderationRequests(results.Items));
    }).WithOpenApi("Handles investigation reconsiderations queries", string.Empty, "investigation_reconsiderations_get")
    .RequireAuthorization()
    .AddGuidValidationQueryParams(["ById"], false)
    .WithParameterValidation();

    endpointRouteBuilder.MapPut("/api/investigation/reconsiderations/submit/{id}", async Task<Results<Ok<string>, BadRequest<ProblemDetails>>> ([FromBody] InvestigationReconsideration investigationReconsideration, [FromRoute] string id, HttpContext ctx, IMediator messagebus, IInvestigationReconsiderationsMapper investigationReconsiderationsMapper, CancellationToken ct) =>
    {
      if (investigationReconsideration.Id != id) return TypedResults.BadRequest(new ProblemDetails()
      {
        Detail = "resource id and payload id do not match"
      });

      var userContext = ctx.User.GetUserContext();

      var command = new InvestigationReconsiderationSubmitCommand(investigationReconsiderationsMapper.MapInvestigationReconsiderationRequest(investigationReconsideration), userContext!.UserId);

      var response = await messagebus.Send(command, ct);

      if (!response.IsSuccess)
      {
        switch (response.ErrorCode)
        {
          case InvestigationReconsiderationSubmitErrorCode.InvestigationReconsiderationNotFound:
            return TypedResults.BadRequest(new ProblemDetails()
            {
              Detail = "investigation reconsideration not found"
            });

          case InvestigationReconsiderationSubmitErrorCode.InvestigationReconsiderationWrongStatus:
            return TypedResults.BadRequest(new ProblemDetails()
            {
              Detail = "investigation reconsideration is in the wrong status"
            });
        }
      }

      return TypedResults.Ok(response.Id);
    }).WithOpenApi("Handles investigation reconsiderations submissions", string.Empty, "investigation_reconsiderations_submit_put")
      .RequireAuthorization()
      .AddGuidValidation("id")
      .WithParameterValidation();
  }
}

/// <summary>
/// Represents a reconsideration request submitted by an applicant.
/// </summary>
public record InvestigationReconsideration()
{
  [ValidGuid]
  public string? Id { get; set; }
  public string? ExplanationAndEvidence { get; set; }
  public InvestigationReconsiderationStatusCode Status { get; set; }

  public InvestigationReconsiderationType Type { get; set; }
  public IEnumerable<Applications.FileInfo> Files { get; set; } = Array.Empty<Applications.FileInfo>();
  public DateTime? ReconsiderationEndDate { get; set; }
}

public enum InvestigationReconsiderationStatusCode
{
  Complete,
  InReview,
  New,
}

public enum InvestigationReconsiderationType
{
  InvestigationOutcome,
  ImmediateAction,
}
