using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPut("/api/draftapplications/{id?}", async Task<Results<Ok<DraftApplicationResponse>, BadRequest<string>>> (string? id, SaveDraftApplicationRequest request, HttpContext httpContext, IMessageBus messageBus, IMapper mapper) =>
        {
          if (request.DraftApplication.Id != id) return TypedResults.BadRequest("resource id and payload id do not match");
          var userContext = httpContext.User.GetUserContext();
          var application = mapper.Map<Managers.Registry.Contract.Applications.Application>(request.DraftApplication, opts => opts.Items.Add("registrantId", userContext!.UserId))!;
          var applicationId = await messageBus.InvokeAsync<string>(new SaveDraftApplicationCommand(application));

          return TypedResults.Ok(new DraftApplicationResponse(applicationId));
        })
        .WithOpenApi("Save a draft application for the current user", string.Empty, "draftapplication_put")
        .RequireAuthorization();

    endpointRouteBuilder.MapPost("/api/applications", async (ApplicationSubmissionRequest request, IMessageBus messageBus) =>
        {
          var cmd = new SubmitApplicationCommand(request.Id);
          await messageBus.InvokeAsync<string>(cmd);

          return TypedResults.Ok();
        })
        .WithOpenApi("Submit an application", string.Empty, "application_post")
        .RequireAuthorization();

    endpointRouteBuilder.MapGet("/api/applications/{id?}", async (string? id, ApplicationStatus[]? byStatus, HttpContext ctx, IMessageBus messageBus, IMapper mapper) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool isNotGuid = !Guid.TryParse(id, out _); if (isNotGuid) { id = null; }
          var query = new ApplicationsQuery
          {
            ById = id,
            ByApplicantId = userId,
            ByStatus = byStatus?.Convert<ApplicationStatus, Managers.Registry.Contract.Applications.ApplicationStatus>()
          };
          var results = await messageBus.InvokeAsync<ApplicationsQueryResults>(query);
          return TypedResults.Ok(mapper.Map<IEnumerable<Application>>(results.Items));
        })
        .WithOpenApi("Handles application queries", string.Empty, "application_get")
        .RequireAuthorization();
  }
}

/// <summary>
/// Save draft application request
/// </summary>
/// <param name="DraftApplication">The draft application</param>
public record SaveDraftApplicationRequest(DraftApplication DraftApplication);

/// <summary>
/// Submit application request
/// </summary>
/// <param name="Id">The application id</param>
public record ApplicationSubmissionRequest(string Id);

/// <summary>
/// Save draft application response
/// </summary>
/// <param name="ApplicationId">The application id</param>
public record DraftApplicationResponse(string ApplicationId);

/// <summary>
/// Application query response
/// </summary>
/// <param name="Items">The found applications</param>
public record ApplicationQueryResponse(IEnumerable<Application> Items);

public record DraftApplication
{
  public string? Id { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
}

public record Application
{
  public string Id { get; set; } = null!;
  public DateTime CreatedOn { get; set; }
  public DateTime? SubmittedOn { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public ApplicationStatus Status { get; set; }
}

public enum CertificationType
{
  [Description("Ece Assistant")]
  EceAssistant,

  [Description("One Year")]
  OneYear,

  [Description("Five Years")]
  FiveYears,

  [Description("Ite")]
  Ite,

  [Description("Sne")]
  Sne,
}

public enum ApplicationStatus
{
  Draft,
  Submitted,
  Complete,
  ReviewforCompletness,
  ReadyforAssessment,
  BeingAssessed,
  Reconsideration,
  Appeal,
  Cancelled,
}
