using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPut("/api/draftapplications/{id?}", async Task<Results<Ok<DraftApplicationResponse>, BadRequest<string>>> (string? id, SaveDraftApplicationRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
        {
          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid && id != null) { id = null; }
          bool ApplicationIdIsNotGuid = !Guid.TryParse(request.DraftApplication.Id, out _); if (ApplicationIdIsNotGuid && request.DraftApplication.Id != null) { request.DraftApplication.Id = null; }

          if (request.DraftApplication.Id != id) return TypedResults.BadRequest("resource id and payload id do not match");
          var userContext = ctx.User.GetUserContext();
          var application = mapper.Map<Managers.Registry.Contract.Applications.Application>(request.DraftApplication, opts => opts.Items.Add("registrantId", userContext!.UserId))!;

          var applicationId = await messageBus.Send(new SaveDraftApplicationCommand(application), ct);
          return TypedResults.Ok(new DraftApplicationResponse(applicationId));
        })
        .WithOpenApi("Save a draft application for the current user", string.Empty, "draftapplication_put")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/applications", async Task<Results<Ok<SubmitApplicationResponse>, BadRequest<ProblemDetails>, NotFound>> (ApplicationSubmissionRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;
          bool IdIsNotGuid = !Guid.TryParse(request.Id, out _); if (IdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ApplicationId is not valid" });
          }

          var cmd = new SubmitApplicationCommand(request.Id, userId!);
          var result = await messageBus.Send(cmd, ct);
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

    endpointRouteBuilder.MapGet("/api/applications/{id?}", async (string? id, ApplicationStatus[]? byStatus, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }
          var query = new ApplicationsQuery
          {
            ById = id,
            ByApplicantId = userId,
            ByStatus = byStatus?.Convert<ApplicationStatus, Managers.Registry.Contract.Applications.ApplicationStatus>()
          };
          var results = await messageBus.Send(query, ct);
          return TypedResults.Ok(mapper.Map<IEnumerable<Application>>(results.Items));
        })
        .WithOpenApi("Handles application queries", string.Empty, "application_get")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/applications/{id}/status", async Task<Results<Ok<SubmittedApplicationStatus>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> (string id, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;
          bool IdIsNotGuid = !Guid.TryParse(id, out _);
          if (IdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ApplicationId is not valid" });
          }
          var query = new ApplicationsQuery
          {
            ById = id,
            ByApplicantId = userId
          };
          var result = await messageBus.Send(query, ct);
          var application = result.Items.FirstOrDefault();
          if (application == null)
          {
            return TypedResults.NotFound(new ProblemDetails() { Title = "Application not found" });
          }
          return TypedResults.Ok(mapper.Map<SubmittedApplicationStatus>(application));
        })
        .WithOpenApi("Handles application status queries", string.Empty, "application_status_get")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapDelete("/api/draftApplications/{id}", async Task<Results<Ok<CancelDraftApplicationResponse>, BadRequest<ProblemDetails>>> (string id, HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ApplicationId is not valid" });
          }

          var cancelledApplicationId = await messageBus.Send(new CancelDraftApplicationCommand(id, userId!), ct);

          return TypedResults.Ok(new CancelDraftApplicationResponse(cancelledApplicationId));
        })
        .WithOpenApi("Cancel a draft application for the current user", "Changes status to cancelled", "draftapplication_delete")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/applications/{application_id}/workexperiencereference/{reference_id?}", async Task<Results<Ok<UpdateReferenceResponse>, BadRequest<ProblemDetails>, NotFound>> (string application_id, string? reference_id, WorkExperienceReference request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;
          bool AppIdIsNotGuid = !Guid.TryParse(application_id, out _);
          if (AppIdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "Application Id is not valid" });
          }

          if (!string.IsNullOrEmpty(reference_id))
          {
            bool RefIdIsNotGuid = !Guid.TryParse(reference_id, out _);
            if (RefIdIsNotGuid)
            {
              return TypedResults.BadRequest(new ProblemDetails() { Title = "Reference Id is not valid" });
            }
          }
          else
          {
            reference_id = "";
          }
          var mappedWorkExperienceReference = mapper.Map<Managers.Registry.Contract.Applications.WorkExperienceReference>(request);
          var cmd = new UpdateWorkExperienceReferenceCommand(mappedWorkExperienceReference, application_id, reference_id, userId!);
          var result = await messageBus.Send(cmd, ct);

          if (!result.IsSuccess)
          {
            var problemDetails = new ProblemDetails
            {
              Status = StatusCodes.Status400BadRequest,
              Title = "Work Experience reference updation failed",
              Extensions = { ["errors"] = result.ErrorMessage }
            };
            return TypedResults.BadRequest(problemDetails);
          }
          return TypedResults.Ok(new UpdateReferenceResponse(result.ReferenceId!));
        })
        .WithOpenApi("Update work experience reference", string.Empty, "application_workexperiencereference_update_post")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/applications/{application_id}/characterreference/{reference_id?}", async Task<Results<Ok<UpdateReferenceResponse>, BadRequest<ProblemDetails>, NotFound>> (string application_id, string? reference_id, CharacterReference request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;
          bool AppIdIsNotGuid = !Guid.TryParse(application_id, out _);
          if (AppIdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "Application Id is not valid" });
          }

          if (!string.IsNullOrEmpty(reference_id))
          {
            bool RefIdIsNotGuid = !Guid.TryParse(reference_id, out _);
            if (RefIdIsNotGuid)
            {
              return TypedResults.BadRequest(new ProblemDetails() { Title = "Reference Id is not valid" });
            }
          }
          else
          {
            reference_id = "";
          }
          var mappedCharacterReference = mapper.Map<Managers.Registry.Contract.Applications.CharacterReference>(request);
          var cmd = new UpdateCharacterReferenceCommand(mappedCharacterReference, application_id, reference_id, userId!);
          var result = await messageBus.Send(cmd, ct);

          if (!result.IsSuccess)
          {
            var problemDetails = new ProblemDetails
            {
              Status = StatusCodes.Status400BadRequest,
              Title = "Character reference updation failed",
              Extensions = { ["errors"] = result.ErrorMessage }
            };
            return TypedResults.BadRequest(problemDetails);
          }
          return TypedResults.Ok(new UpdateReferenceResponse(result.ReferenceId!));
        })
        .WithOpenApi("Update character reference", string.Empty, "application_characterreference_update_post")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/applications/{applicationId}/characterReference/{referenceId}/resendInvite", async Task<Results<Ok<ResendReferenceInviteResponse>, BadRequest<ProblemDetails>>> (string applicationId, string referenceId, HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool IdIsNotGuid = !Guid.TryParse(applicationId, out _);
          if (IdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ApplicationId is not a valid guid" });
          }

          bool ReferenceIdIsNotGuid = !Guid.TryParse(referenceId, out _);
          if (ReferenceIdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ReferenceId is not a valid guid" });
          }

          return TypedResults.Ok(new ResendReferenceInviteResponse(await messageBus.Send(new ResendCharacterReferenceInviteRequest(applicationId, referenceId, userId!), ct)));
        })
        .WithOpenApi("Resend a character reference invite", "Changes character reference invite again status to true", "application_character_reference_resend_invite_post")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/applications/{applicationId}/workExperienceReference/{referenceId}/resendInvite", async Task<Results<Ok<ResendReferenceInviteResponse>, BadRequest<ProblemDetails>>> (string applicationId, string referenceId, HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool IdIsNotGuid = !Guid.TryParse(applicationId, out _);
          if (IdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ApplicationId is not a valid guid" });
          }

          bool ReferenceIdIsNotGuid = !Guid.TryParse(referenceId, out _);
          if (ReferenceIdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ReferenceId is not a valid guid" });
          }

          return TypedResults.Ok(new ResendReferenceInviteResponse(await messageBus.Send(new ResendWorkExperienceReferenceInviteRequest(applicationId, referenceId, userId!), ct)));
        })
        .WithOpenApi("Resend a work experience reference invite", "Changes work experience reference invite again status to true", "application_work_experience_reference_resend_invite_post")
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
/// Save draft application response
/// </summary>
/// <param name="ApplicationId">The application id</param>
public record DraftApplicationResponse(string ApplicationId);

/// <summary>
/// delete draft application response
/// </summary>
/// <param name="ApplicationId">The application id</param>
public record CancelDraftApplicationResponse(string ApplicationId);

public record SubmitApplicationResponse(string ApplicationId);

public record UpdateReferenceResponse(string ReferenceId);

/// <summary>
/// Application query response
/// </summary>
/// <param name="Items">The found applications</param>
public record ApplicationQueryResponse(IEnumerable<Application> Items);

/// <summary>
/// Resend reference invite response
/// </summary>
/// <param name="ReferenceId">The reference id</param>
public record ResendReferenceInviteResponse(string ReferenceId);

public record DraftApplication
{
  public string? Id { get; set; }
  public DateTime? SignedDate { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
  public IEnumerable<ProfessionalDevelopment> ProfessionalDevelopments { get; set; } = Array.Empty<ProfessionalDevelopment>();
  public string? Stage { get; set; }
  public ApplicationTypes ApplicationType { get; set; }
  public EducationOrigin? EducationOrigin { get; set; }
  public EducationRecognition? EducationRecognition { get; set; }
  public string? ExplanationLetter { get; set; }
  public OneYearRenewalexplanations OneYearRenewalexplanation { get; set; }
  public DateTime CreatedOn { get; set; }
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
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
  public IEnumerable<ProfessionalDevelopment> ProfessionalDevelopments { get; set; } = Array.Empty<ProfessionalDevelopment>();
  public ApplicationStatus Status { get; set; }
  public string? Stage { get; set; }
  public ApplicationTypes ApplicationType { get; set; }
  public EducationOrigin? EducationOrigin { get; set; }
  public EducationRecognition? EducationRecognition { get; set; }
  public string? ExplanationLetter { get; set; }
  public OneYearRenewalexplanations OneYearRenewalexplanation { get; set; }
}
public record ProfessionalDevelopment([Required] string CertificationNumber, [Required] DateTime CertificationExpiryDate, [Required] DateTime DateSigned, [Required] string CourseName, [Required] string OrganizationName, [Required] DateTime StartDate, [Required] DateTime EndDate)
{
  public string? Id { get; set; }
  public string? OrganizationContactInformation { get; set; }
  public string? InstructorName { get; set; }
  [Required]
  public int? NumberOfHours { get; set; }
  public ProfessionalDevelopmentStatusCode? Status { get; set; }
  public IEnumerable<string> ToBeDeletedFileIds { get; set; } = Array.Empty<string>();
  public IEnumerable<string> ToBeAddedFileIds { get; set; } = Array.Empty<string>();
  public IEnumerable<string> Files { get; set; } = Array.Empty<string>();
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
  public bool IsECEAssistant { get; set; }
  public bool DoesECERegistryHaveTranscript { get; set; }
  public bool IsOfficialTranscriptRequested { get; set; }
}
public record WorkExperienceReference([Required] string FirstName, [Required] string LastName, [Required] string EmailAddress, [Required] int Hours)
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

public enum OneYearRenewalexplanations
{
  Icouldnotfindemploymenttocompletetherequiredhours,
  Icouldnotworkduetomyvisastatusstudentvisaexpiredvisa,
  IliveandworkinacommunitywithoutothercertifiedECEs,
  Iwasunabletoenterthecountryasexpected,
  Iwasunabletoworkinthechildcarefieldforpersonalreasons,
  Other,
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
  Pending,
  Ready,
  InProgress,
  PendingQueue,
  ReconsiderationDecision,
  AppealDecision
}

public enum ApplicationStatusReasonDetail
{
  Actioned,
  BeingAssessed,
  Certified,
  Denied,
  ForReview,
  InvestigationsConsultationNeeded,
  MoreInformationRequired,
  OperationSupervisorManagerofCertificationsConsultationNeeded,
  PendingDocuments,
  ProgramAnalystReview,
  ReadyforAssessment,
  ReceivedPending,
  ReceivePhysicalTranscripts,
  SupervisorConsultationNeeded,
  ValidatingIDs,
}

public enum ApplicationTypes
{
  New,
  Renewal,
  LaborMobility,
}

public enum EducationOrigin
{
  InsideBC,
  OutsideBC,
  OutsideofCanada
}

public enum EducationRecognition
{
  Recognized,
  NotRecognized
}

public record CharacterReference([Required] string FirstName, [Required] string LastName, string? PhoneNumber, [Required] string EmailAddress)
{
  public string? Id { get; set; }
}

public record SubmittedApplicationStatus(string Id, DateTime SubmittedOn, ApplicationStatus Status, ApplicationStatusReasonDetail SubStatus)
{
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public DateTime? ReadyForAssessmentDate { get; set; }
  public IEnumerable<TranscriptStatus> TranscriptsStatus { get; set; } = Array.Empty<TranscriptStatus>();
  public IEnumerable<WorkExperienceReferenceStatus> WorkExperienceReferencesStatus { get; set; } = Array.Empty<WorkExperienceReferenceStatus>();
  public IEnumerable<CharacterReferenceStatus> CharacterReferencesStatus { get; set; } = Array.Empty<CharacterReferenceStatus>();
  public bool? AddMoreCharacterReference { get; set; }
  public bool? AddMoreWorkExperienceReference { get; set; }
}

public record TranscriptStatus(string Id, TranscriptStage Status, string EducationalInstitutionName);

public record WorkExperienceReferenceStatus(string Id, WorkExperienceRefStage Status, string FirstName, string LastName, string EmailAddress)
{
  public string? PhoneNumber { get; set; }
  public int? TotalNumberofHoursAnticipated { get; set; }
  public int? TotalNumberofHoursApproved { get; set; }
  public int? TotalNumberofHoursObserved { get; set; }
  public bool? WillProvideReference { get; set; }
}

public record CharacterReferenceStatus(string Id, CharacterReferenceStage Status, string FirstName, string LastName, string EmailAddress)
{
  public string? PhoneNumber { get; set; }
  public bool? WillProvideReference { get; set; }
}

public enum TranscriptStage
{
  Accepted,
  ApplicationSubmitted,
  Draft,
  InProgress,
  Rejected,
  Submitted,
  WaitingforDetails
}

public enum WorkExperienceRefStage
{
  ApplicationSubmitted,
  Approved,
  Draft,
  InProgress,
  Rejected,
  Submitted,
  UnderReview,
  WaitingforResponse
}

public enum CharacterReferenceStage
{
  ApplicationSubmitted,
  Approved,
  Draft,
  InProgress,
  Rejected,
  Submitted,
  UnderReview,
  WaitingResponse
}

public enum ProfessionalDevelopmentStatusCode
{
  ApplicationSubmitted,
  Draft,
  Inactive,
  InProgress,
  Rejected,
  Submitted,
  UnderReview,
  WaitingResponse,
}
