﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPut("/api/draftapplications/{id?}", async Task<Results<Ok<DraftApplicationResponse>, BadRequest<string>, NotFound>> (string? id, SaveDraftApplicationRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
        {
          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid && id != null) { id = null; }
          bool ApplicationIdIsNotGuid = !Guid.TryParse(request.DraftApplication.Id, out _); if (ApplicationIdIsNotGuid && request.DraftApplication.Id != null) { request.DraftApplication.Id = null; }

          if (request.DraftApplication.Id != id) return TypedResults.BadRequest("resource id and payload id do not match");
          var userContext = ctx.User.GetUserContext();
          var draftApplication = mapper.Map<Managers.Registry.Contract.Applications.Application>(request.DraftApplication, opts => opts.Items.Add("registrantId", userContext!.UserId))!;

          if (id != null)
          {
            var query = new ApplicationsQuery
            {
              ById = id,
              ByApplicantId = userContext!.UserId,
              ByStatus = [Managers.Registry.Contract.Applications.ApplicationStatus.Draft]
            };
            var results = await messageBus.Send(query, ct);
            if (!results.Items.Any()) return TypedResults.NotFound();
          }

          var application = await messageBus.Send(new SaveDraftApplicationCommand(draftApplication), ct);
          return TypedResults.Ok(new DraftApplicationResponse(mapper.Map<Application>(application)));
        })
        .WithOpenApi("Save a draft application for the current user", string.Empty, "draftapplication_put")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/applications", async Task<Results<Ok<SubmitApplicationResponse>, BadRequest<ProblemDetails>, NotFound>> (ApplicationSubmissionRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
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
          return TypedResults.Ok(new SubmitApplicationResponse(mapper.Map<Application>(result.Application)));
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

    endpointRouteBuilder.MapPost("/api/applications/{application_id}/professionaldevelopment/add", async Task<Results<Ok<AddProfessionalDevelopmentResponse>, BadRequest<ProblemDetails>, NotFound>> (string application_id, ProfessionalDevelopment request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;
          bool AppIdIsNotGuid = !Guid.TryParse(application_id, out _);
          if (AppIdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "Application Id is not valid" });
          }

          var mappedProfessionalDevelopment = mapper.Map<Managers.Registry.Contract.Applications.ProfessionalDevelopment>(request);
          var cmd = new AddProfessionalDevelopmentCommand(mappedProfessionalDevelopment, application_id, userId!);
          var result = await messageBus.Send(cmd, ct);

          if (!result.IsSuccess)
          {
            var problemDetails = new ProblemDetails
            {
              Status = StatusCodes.Status400BadRequest,
              Title = "Adding Professional Development failed",
              Extensions = { ["errors"] = result.ErrorMessage }
            };
            return TypedResults.BadRequest(problemDetails);
          }
          return TypedResults.Ok(new AddProfessionalDevelopmentResponse(result.ApplicationId!));
        })
       .WithOpenApi("Add Professional Development", string.Empty, "application_professionaldevelopment_add_post")
       .RequireAuthorization()
       .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/applications/{application_id}/transcriptDocuments", async Task<Results<Ok<DraftApplicationResponse>, BadRequest<string>>> (TranscriptDocuments request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      bool ApplicationIdIsNotGuid = !Guid.TryParse(request.ApplicationId, out _);
      if (ApplicationIdIsNotGuid)
      {
        return TypedResults.BadRequest("ApplicationId is not guid");
      }

      bool TranscriptIdIsNotGuid = !Guid.TryParse(request.TranscriptId, out _);
      if (TranscriptIdIsNotGuid)
      {
        return TypedResults.BadRequest("TranscriptId is not guid");
      }

      var userContext = ctx.User.GetUserContext();
      var transcriptDocuments = mapper.Map<Managers.Registry.Contract.Applications.TranscriptDocuments>(request, opts => opts.Items.Add("RegistrantId", userContext!.UserId))!;

      var application = await messageBus.Send(new SaveApplicationTranscriptCommand(transcriptDocuments), ct);
      return TypedResults.Ok(new DraftApplicationResponse(mapper.Map<Application>(application)));
    })
    .WithOpenApi("Save application transcript documents and options", string.Empty, "application_update_transcript_post")
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
/// <param name="Application">The application</param>
public record DraftApplicationResponse(Application Application);

/// <summary>
/// delete draft application response
/// </summary>
/// <param name="ApplicationId">The application id</param>
public record CancelDraftApplicationResponse(string ApplicationId);

public record SubmitApplicationResponse(Application Application);

public record UpdateReferenceResponse(string ReferenceId);

public record AddProfessionalDevelopmentResponse(string ApplicationId);

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
  public OneYearRenewalexplanations? OneYearRenewalExplanationChoice { get; set; }
  public FiveYearRenewalExplanations? FiveYearRenewalExplanationChoice { get; set; }
  public string? RenewalExplanationOther { get; set; }
  public DateTime? CreatedOn { get; set; }
  public ApplicationOrigin? Origin { get; set; }
  public CertificateInformation? LabourMobilityCertificateInformation { get; set; }
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
  public OneYearRenewalexplanations? OneYearRenewalExplanationChoice { get; set; }
  public FiveYearRenewalExplanations? FiveYearRenewalExplanationChoice { get; set; }
  public string? RenewalExplanationOther { get; set; }
  public ApplicationOrigin? Origin { get; set; }
  public CertificateInformation? LabourMobilityCertificateInformation { get; set; }
}
public record ProfessionalDevelopment([Required] string CourseName, [Required] string OrganizationName, [Required] DateTime StartDate, [Required] DateTime EndDate, [Required] double NumberOfHours)
{
  public string? Id { get; set; }
  public string? OrganizationContactInformation { get; set; }
  public string? OrganizationEmailAddress { get; set; }
  public string? InstructorName { get; set; }
  public string? CourseorWorkshopLink { get; set; }
  public ProfessionalDevelopmentStatusCode? Status { get; set; }
  public IEnumerable<string> DeletedFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<string> NewFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<FileInfo> Files { get; set; } = Array.Empty<FileInfo>();
}
public record Transcript(string? EducationalInstitutionName, [Required] string ProgramName, [Required] string StudentLastName, [Required] DateTime StartDate, [Required] DateTime EndDate, [Required] bool IsNameUnverified, [Required] EducationRecognition EducationRecognition, [Required] EducationOrigin EducationOrigin)
{
  public string? Id { get; set; }
  public string? CampusLocation { get; set; }
  public string StudentFirstName { get; set; } = string.Empty;
  public string? StudentMiddleName { get; set; }
  public string? StudentNumber { get; set; }
  public bool IsECEAssistant { get; set; }
  public TranscriptStatusOptions? TranscriptStatusOption { get; set; }
  public Country? Country { get; set; }
  public Province? Province { get; set; }
  public PostSecondaryInstitution? PostSecondaryInstitution { get; set; }
}
public record WorkExperienceReference([Required] string LastName, [Required] string EmailAddress, [Required] int Hours)
{
  public string? FirstName { get; set; }
  public string? Id { get; set; }
  public string? PhoneNumber { get; set; }
  public WorkExperienceTypes? Type { get; set; }
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

public enum FiveYearRenewalExplanations
{
  Ileftthechildcarefieldforpersonalreasons,
  Iwasunabletocompletetherequiredhoursofprofessionaldevelopment,
  Iwasunabletofindemploymentinthechildcarefieldinmycommunity,
  MyemploymentdiddoesnotrequirecertificationasanECEforexamplenannyteachercollegeinstructoradministratoretc,
  Other,
}

public enum OneYearRenewalexplanations
{
  IliveandworkinacommunitywithoutothercertifiedECEs,
  Iwasunabletofindemploymentinthechildcarefieldtocompletetherequirednumberofhours,
  Iwasunabletoworkduetothestatusofmyvisaorwasunabletoenterthecountryasexpected,
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
  Closed,
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

public enum ApplicationOrigin
{
  Manual,
  Oracle,
  Portal
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
  LabourMobility,
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

public record CharacterReference([Required] string LastName, string? PhoneNumber, [Required] string EmailAddress)
{
  public string? FirstName { get; set; }
  public string? Id { get; set; }
}

public record SubmittedApplicationStatus(string Id, DateTime SubmittedOn, ApplicationStatus Status, ApplicationStatusReasonDetail SubStatus)
{
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public DateTime? ReadyForAssessmentDate { get; set; }
  public IEnumerable<TranscriptStatus> TranscriptsStatus { get; set; } = Array.Empty<TranscriptStatus>();
  public IEnumerable<WorkExperienceReferenceStatus> WorkExperienceReferencesStatus { get; set; } = Array.Empty<WorkExperienceReferenceStatus>();
  public IEnumerable<CharacterReferenceStatus> CharacterReferencesStatus { get; set; } = Array.Empty<CharacterReferenceStatus>();
  public IEnumerable<ProfessionalDevelopmentStatus> ProfessionalDevelopmentsStatus { get; set; } = Array.Empty<ProfessionalDevelopmentStatus>();
  public bool? AddMoreCharacterReference { get; set; }
  public bool? AddMoreWorkExperienceReference { get; set; }
  public bool? AddMoreProfessionalDevelopment { get; set; }
  public ApplicationTypes? ApplicationType { get; set; }
}
public record FileInfo(string Id)
{
  public string? Url { get; set; } = string.Empty;
  public string? Extention { get; set; } = string.Empty;
  public string? Name { get; set; } = string.Empty;
  public string? Size { get; set; } = string.Empty;
}
public record TranscriptStatus(string Id, TranscriptStage Status, string EducationalInstitutionName, string programName)
{
  public IEnumerable<FileInfo> CourseOutlineFiles { get; set; } = Array.Empty<FileInfo>();
  public IEnumerable<FileInfo> ProgramConfirmationFiles { get; set; } = Array.Empty<FileInfo>();
  public CourseOutlineOptions? CourseOutlineOptions { get; set; }
  public ComprehensiveReportOptions? ComprehensiveReportOptions { get; set; }
  public ProgramConfirmationOptions? ProgramConfirmationOptions { get; set; }
  public bool? CourseOutlineReceivedByRegistry { get; set; }
  public bool? ProgramConfirmationReceivedByRegistry { get; set; }
  public bool? TranscriptReceivedByRegistry { get; set; }
  public bool? ComprehensiveReportReceivedByRegistry { get; set; }
  public Country? Country { get; set; }
  public EducationRecognition EducationRecognition { get; set; }
}

public record WorkExperienceReferenceStatus(string Id, WorkExperienceRefStage Status, string FirstName, string LastName, string EmailAddress)
{
  public string? PhoneNumber { get; set; }
  public int? TotalNumberofHoursAnticipated { get; set; }
  public int? TotalNumberofHoursApproved { get; set; }
  public int? TotalNumberofHoursObserved { get; set; }
  public bool? WillProvideReference { get; set; }
  public WorkExperienceTypes? Type { get; set; }
}

public record CharacterReferenceStatus(string Id, CharacterReferenceStage Status, string FirstName, string LastName, string EmailAddress)
{
  public string? PhoneNumber { get; set; }
  public bool? WillProvideReference { get; set; }
}

public record ProfessionalDevelopmentStatus(string Id, string CourseName, int NumberOfHours)
{
  public ProfessionalDevelopmentStatusCode? Status { get; set; }
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

public enum WorkExperienceTypes
{
  Is400Hours,
  Is500Hours,
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
  Approved,
  Draft,
  InProgress,
  Rejected,
  Submitted,
  UnderReview,
  WaitingResponse,
}

public record TranscriptDocuments(string ApplicationId, string TranscriptId)
{
  public IEnumerable<string> NewCourseOutlineFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<string> NewProgramConfirmationFiles { get; set; } = Array.Empty<string>();
  public CourseOutlineOptions? CourseOutlineOptions { get; set; }
  public ComprehensiveReportOptions? ComprehensiveReportOptions { get; set; }
  public ProgramConfirmationOptions? ProgramConfirmationOptions { get; set; }
}

public enum CourseOutlineOptions
{
  UploadNow,
  RegistryAlreadyHas
}

public enum ComprehensiveReportOptions
{
  FeeWaiver,
  InternationalCredentialEvaluationService,
  RegistryAlreadyHas
}

public enum ProgramConfirmationOptions
{
  UploadNow,
  RegistryAlreadyHas
}

public enum TranscriptStatusOptions
{
  RegistryHasTranscript,
  OfficialTranscriptRequested,
  TranscriptWillRequireEnglishTranslation
}

public record CertificateInformation
{
  public string? CertificateComparisonId { get; set; }
  public Province? LabourMobilityProvince { get; set; }
  public string? CurrentCertificationNumber { get; set; } = string.Empty;
  public string? ExistingCertificationType { get; set; } = string.Empty;
  public string? LegalFirstName { get; set; } = string.Empty;
  public string? LegalMiddleName { get; set; } = string.Empty;
  public string? LegalLastName { get; set; } = string.Empty;
  public bool? HasOtherName { get; set; }
}
