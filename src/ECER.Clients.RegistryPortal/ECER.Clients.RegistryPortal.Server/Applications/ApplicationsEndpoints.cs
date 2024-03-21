using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPut("/api/draftapplications/{id?}", async Task<Results<Ok<DraftApplicationResponse>, BadRequest<string>>> (string? id, SaveDraftApplicationRequest request, HttpContext ctx, CancellationToken ct, IMessageBus messageBus, IMapper mapper) =>
        {
          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid && id != null) { id = null; }
          bool ApplicationIdIsNotGuid = !Guid.TryParse(request.DraftApplication.Id, out _); if (ApplicationIdIsNotGuid && request.DraftApplication.Id != null) { request.DraftApplication.Id = null; }

          if (request.DraftApplication.Id != id) return TypedResults.BadRequest("resource id and payload id do not match");
          var userContext = ctx.User.GetUserContext();
          var application = mapper.Map<Managers.Registry.Contract.Applications.Application>(request.DraftApplication, opts => opts.Items.Add("registrantId", userContext!.UserId))!;

          var applicationId = await messageBus.InvokeAsync<string>(new SaveDraftApplicationCommand(application), ct);
          return TypedResults.Ok(new DraftApplicationResponse(applicationId));
        })
        .WithOpenApi("Save a draft application for the current user", string.Empty, "draftapplication_put")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/applications", async Task<Results<Ok<SubmitApplicationResponse>, BadRequest<ProblemDetails>, NotFound>> (ApplicationSubmissionRequest request, HttpContext ctx, CancellationToken ct, IMessageBus messageBus) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;
          bool IdIsNotGuid = !Guid.TryParse(request.Id, out _); if (IdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ApplicationId is not valid" });
          }

          var cmd = new SubmitApplicationCommand(request.Id, userId!);
          var result = await messageBus.InvokeAsync<ApplicationSubmissionResult>(cmd, ct);
          if (!result.IsSuccess && result.Error == SubmissionError.DraftApplicationNotFound)
          {
            return TypedResults.NotFound();
          }
          if (!result.IsSuccess && result.Error == SubmissionError.DraftApplicationValidationFailed)
          {
            var problemDetails = new ProblemDetails
            {
              Status = StatusCodes.Status400BadRequest,
              Title = "Application submission failed",
              Extensions = { ["errors"] = result.ValidationErrors }
            };
            return TypedResults.BadRequest(problemDetails);
          }
          return TypedResults.Ok(new SubmitApplicationResponse(result.ApplicationId!));
        })
        .WithOpenApi("Submit an application", string.Empty, "application_post")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/applications/{id?}", async (string? id, ApplicationStatus[]? byStatus, HttpContext ctx, IMessageBus messageBus, IMapper mapper) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }
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
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapDelete("/api/draftApplications/{id}", async Task<Results<Ok<DeleteDraftApplicationResponse>, BadRequest<ProblemDetails>>> (string id, HttpContext ctx, CancellationToken ct, IMessageBus messageBus) =>
       {
         var userId = ctx.User.GetUserContext()?.UserId;

         bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid)
         {
           return TypedResults.BadRequest(new ProblemDetails() { Title = "ApplicationId is not valid" });
         }

         var deletedApplicationId = await messageBus.InvokeAsync<string>(new DeleteDraftApplicationCommand(id, userId!, ct));

         return TypedResults.Ok(new DeleteDraftApplicationResponse(deletedApplicationId));
       })
       .WithOpenApi("Delete a draft application for the current user", "Changes status to cancelled", "draftapplication_delete")
       .RequireAuthorization()
       .WithParameterValidation();
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
/// delete draft application request
/// </summary>
/// <param name="Id">The application id</param>
public record DeleteDraftApplicationRequest(string Id);

/// <summary>
/// Save draft application response
/// </summary>
/// <param name="Id">The application id</param>
public record DraftApplicationResponse(string Id);

/// <summary>
/// delete draft application response
/// </summary>
/// <param name="ApplicationId">The application id</param>
public record DeleteDraftApplicationResponse(string ApplicationId);

public record SubmitApplicationResponse(string ApplicationId);

/// <summary>
/// Application query response
/// </summary>
/// <param name="Items">The found applications</param>
public record ApplicationQueryResponse(IEnumerable<Application> Items);

public record DraftApplication
{
  public string? Id { get; set; }
  public DateTime? SignedDate { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
  public PortalStage Stage { get; set; }
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
  public ApplicationStatus Status { get; set; }

}

public record Application
{
  public string Id { get; set; } = null!;
  public DateTime CreatedOn { get; set; }
  public DateTime? SubmittedOn { get; set; }
  public DateTime? SignedDate { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
  public ApplicationStatus Status { get; set; }
  public PortalStage Stage { get; set; }
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
}

public record Transcript()
{
  public string? Id { get; set; }
  [Required]
  public string? EducationalInstitutionName { get; set; }
  [Required]
  public string? ProgramName { get; set; }
  public string? CampusLocation { get; set; }
  [Required]
  public string? StudentName { get; set; }
  [Required]
  public string? StudentNumber { get; set; }
  public string? LanguageofInstruction { get; set; }
  [Required]
  public DateTime StartDate { get; set; }
  [Required]
  public DateTime EndDate { get; set; }
}
public record WorkExperienceReference([Required] string? FirstName, [Required] string? LastName, [Required] string? EmailAddress, [Required] int? Hours)
{
  public string? Id { get; set; }

  public string? PhoneNumber { get; set; }
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

public enum PortalStage
{
  CertificationType,
  Declaration,
  ContactInformation,
  Education,
  CharacterReferences,
  WorkReferences,
  Review,
}

public enum ApplicationStatus
{
  Draft,
  Submitted,
  Complete,
  Reconsideration,
  Cancelled,
  Escalated,
  Decision,
  Withdrawn,
  Ready,
  InProgress,
  PendingQueue,
  ReconsiderationDecision
}

public record CharacterReference([Required] string? FirstName, [Required] string? LastName, string? PhoneNumber, [Required] string? EmailAddress)
{
  public string? Id { get; set; }
}
