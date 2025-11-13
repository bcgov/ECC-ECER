using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.ICRA;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.RegistryPortal.Server.ICRA;

public class ICRAEligibilitiesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPut("/api/icra/{id?}", async Task<Results<Ok<DraftICRAEligibilityResponse>, BadRequest<string>, NotFound>> (string? id, SaveDraftICRAEligibilityRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
        {
          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid && id != null) { id = null; }
          bool PayloadIdIsNotGuid = !Guid.TryParse(request.Eligibility.Id, out _); if (PayloadIdIsNotGuid && request.Eligibility.Id != null) { request.Eligibility.Id = null; }

          if (request.Eligibility.Id != id) return TypedResults.BadRequest("resource id and payload id do not match");
          var userContext = ctx.User.GetUserContext();
          request.Eligibility.ApplicantId = userContext!.UserId;
          var eligibility = mapper.Map<Managers.Registry.Contract.ICRA.ICRAEligibility>(request.Eligibility)!;

          if (id != null)
          {
            var query = new ICRAEligibilitiesQuery
            {
              ById = id,
              ByApplicantId = userContext!.UserId,
              ByStatus = [Managers.Registry.Contract.ICRA.ICRAStatus.Draft]
            };
            var results = await messageBus.Send(query, ct);
            if (!results.Items.Any()) return TypedResults.NotFound();
          }

          var freshEligibility = await messageBus.Send(new SaveICRAEligibilityCommand(eligibility), ct);
          return TypedResults.Ok(new DraftICRAEligibilityResponse(mapper.Map<ICRAEligibility>(freshEligibility)));
        })
        .WithOpenApi("Save a draft icra eligibility for the current user", string.Empty, "icra_put")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/icra/{id?}", async (string? id, ICRAStatus[]? byStatus, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }
          var query = new ICRAEligibilitiesQuery
          {
            ById = id,
            ByApplicantId = userId,
            ByStatus = byStatus?.Convert<ICRAStatus, Managers.Registry.Contract.ICRA.ICRAStatus>()
          };
          var results = await messageBus.Send(query, ct);
          return TypedResults.Ok(mapper.Map<IEnumerable<ICRAEligibility>>(results.Items));
        })
        .WithOpenApi("Handles icra queries", string.Empty, "icra_get")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/icra", async Task<Results<Ok<SubmitICRAEligibilityResponse>, BadRequest<ProblemDetails>, NotFound>> (ICRAEligibilitySubmissionRequest request, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;
          bool IdIsNotGuid = !Guid.TryParse(request.Id, out _); if (IdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ICRAEligibilityId is not valid" });
          }

          var cmd = new SubmitICRAEligibilityCommand(request.Id!, userId!);
          var result = await messageBus.Send(cmd, ct);
          if (!result.IsSuccess && result.Error == Managers.Registry.Contract.ICRA.SubmissionError.DraftIcraEligibilityNotFound)
          {
            return TypedResults.NotFound();
          }
          if (!result.IsSuccess && result.Error == Managers.Registry.Contract.ICRA.SubmissionError.DraftIcraEligibilityValidationFailed)
          {
            var problemDetails = new ProblemDetails
            {
              Status = StatusCodes.Status400BadRequest,
              Title = "ICRA Eligibility submission failed",
              Extensions = { ["errors"] = result.ValidationErrors }
            };
            return TypedResults.BadRequest(problemDetails);
          }
          return TypedResults.Ok(new SubmitICRAEligibilityResponse(mapper.Map<ICRAEligibility>(result.Eligibility)));
        })
        .WithOpenApi("Submit an ICRA Eligibility", string.Empty, "icra_post")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/icra/{id}/status", async Task<Results<Ok<ICRAEligibilityStatus>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> (string id, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;
          bool IdIsNotGuid = !Guid.TryParse(id, out _);
          if (IdIsNotGuid)
          {
            return TypedResults.BadRequest(new ProblemDetails() { Title = "ICRAEligibilityId is not valid" });
          }

          var query = new ICRAEligibilitiesQuery
          {
            ById = id,
            ByApplicantId = userId
          };
          var result = await messageBus.Send(query, ct);
          var eligibility = result.Items.FirstOrDefault();
          if (eligibility == null)
          {
            return TypedResults.NotFound(new ProblemDetails() { Title = "ICRA eligibility not found" });
          }
          return TypedResults.Ok(mapper.Map<ICRAEligibilityStatus>(eligibility));
        })
        .WithOpenApi("Handles icra eligibility status queries", string.Empty, "icra_status_get")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/icra/{icraEligibilityId}/workReference/{referenceId}/resendInvite", async Task<Results<Ok<ResendIcraReferenceInviteResponse>, BadRequest<ProblemDetails>>> (string icraEligibilityId, string referenceId, HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
    {
      var userId = ctx.User.GetUserContext()?.UserId;

      bool IdIsNotGuid = !Guid.TryParse(icraEligibilityId, out _);
      if (IdIsNotGuid)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Title = "icraEligibilityId is not a valid guid" });
      }

      bool ReferenceIdIsNotGuid = !Guid.TryParse(referenceId, out _);
      if (ReferenceIdIsNotGuid)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Title = "ReferenceId is not a valid guid" });
      }

      return TypedResults.Ok(new ResendIcraReferenceInviteResponse(await messageBus.Send(new ResendIcraWorkExperienceReferenceInviteCommand(icraEligibilityId, referenceId, userId!), ct)));
    })
    .WithOpenApi("Resend an icra work reference invite", "Changes work reference invite again status to true", "icra_work_reference_resend_invite_post")
    .RequireAuthorization()
    .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/icra/{icraEligibilityId}/workReference/add", async Task<Results<Ok<EmploymentReference>, BadRequest<ProblemDetails>, NotFound>> (string icraEligibilityId, EmploymentReference employmentReference, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      var userId = ctx.User.GetUserContext()?.UserId;
      bool AppIdIsNotGuid = !Guid.TryParse(icraEligibilityId, out _);
      if (AppIdIsNotGuid)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Title = "Application Id is not valid" });
      }
      var mappedWorkExperienceReference = mapper.Map<Managers.Registry.Contract.ICRA.EmploymentReference>(employmentReference);
      var cmd = new AddIcraWorkExperienceReferenceCommand(mappedWorkExperienceReference, icraEligibilityId, userId!);
      var result = await messageBus.Send(cmd, ct);

      if (!result.IsSuccess)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Title = "Icra work experience reference add failed",
          Extensions = { ["errors"] = result.ErrorMessage }
        };
        return TypedResults.BadRequest(problemDetails);
      }
      return TypedResults.Ok(mapper.Map<EmploymentReference>(result.EmploymentReference));
    })
    .WithOpenApi("Update work experience reference", string.Empty, "icra_work_reference_add_post")
    .RequireAuthorization()
    .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/icra/{icraEligibilityId}/workReference/{referenceId}/replace", async Task<Results<Ok<EmploymentReference>, BadRequest<ProblemDetails>, NotFound>> (string icraEligibilityId, string referenceId, EmploymentReference employmentReference, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      var userId = ctx.User.GetUserContext()?.UserId;
      bool AppIdIsNotGuid = !Guid.TryParse(icraEligibilityId, out _);
      if (AppIdIsNotGuid)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Title = "Application Id is not valid" });
      }

      bool ReferenceIdIsNotGuid = !Guid.TryParse(referenceId, out _);
      if (ReferenceIdIsNotGuid)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Title = "ReferenceId is not a valid guid" });
      }

      var mappedWorkExperienceReference = mapper.Map<Managers.Registry.Contract.ICRA.EmploymentReference>(employmentReference);
      var cmd = new ReplaceIcraWorkExperienceReferenceCommand(mappedWorkExperienceReference, icraEligibilityId, referenceId, userId!);
      var result = await messageBus.Send(cmd, ct);

      if (!result.IsSuccess)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Title = "Icra work experience reference replace failed",
          Extensions = { ["errors"] = result.ErrorMessage }
        };
        return TypedResults.BadRequest(problemDetails);
      }
      return TypedResults.Ok(mapper.Map<EmploymentReference>(result.EmploymentReference));
    })
    .WithOpenApi("Replace work experience reference", string.Empty, "icra_work_reference_replace_post")
    .RequireAuthorization()
    .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/icra/workReference/{workExperienceReferenceId}", async Task<Results<Ok<EmploymentReference>, BadRequest<ProblemDetails>, NotFound>> (string workExperienceReferenceId, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var userId = ctx.User.GetUserContext()?.UserId;

      bool IdIsNotGuid = !Guid.TryParse(workExperienceReferenceId, out _); if (IdIsNotGuid)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Title = "Work reference id is not a valid guid" });
      }

      var cmd = new GetIcraWorkExperienceReferenceByIdCommand(workExperienceReferenceId, userId!);

      var result = await messageBus.Send(cmd, ct);

      if (result == null)
      {
        return TypedResults.NotFound();
      }
      return TypedResults.Ok(mapper.Map<EmploymentReference>(result));
    })

    .WithOpenApi("Get work experience reference by id", string.Empty, "icra_work_reference_by_id_get")
    .RequireAuthorization()
    .WithParameterValidation();
  }
}

public record SaveDraftICRAEligibilityRequest(ICRAEligibility Eligibility);

public record DraftICRAEligibilityResponse(ICRAEligibility Eligibility);

public record ICRAEligibilityQueryResponse(IEnumerable<ICRAEligibility> Items);

public record ICRAEligibility()
{
  public string? Id { get; set; }
  public string ApplicantId { get; set; } = string.Empty;
  public string? PortalStage { get; set; }
  public DateTime? SignedDate { get; set; }
  public DateTime? CreatedOn { get; set; }
  public ICRAStatus Status { get; set; }
  public IEnumerable<InternationalCertification> InternationalCertifications { get; set; } = Array.Empty<InternationalCertification>();
  public IEnumerable<EmploymentReference> EmploymentReferences { get; set; } = Array.Empty<EmploymentReference>();
  public bool AddAdditionalEmploymentExperienceReferences { get; set; }
}
public record InternationalCertification
{
  public string? Id { get; set; }
  public string? OtherFirstName { get; set; }
  public string? OtherMiddleName { get; set; }
  public string? OtherLastName { get; set; }
  public bool HasOtherName { get; set; }
  public string? CountryId { get; set; }
  public string? NameOfRegulatoryAuthority { get; set; }
  public string? EmailOfRegulatoryAuthority { get; set; }
  public string? PhoneOfRegulatoryAuthority { get; set; }
  public string? WebsiteOfRegulatoryAuthority { get; set; }
  public string? OnlineCertificateValidationToolOfRegulatoryAuthority { get; set; }
  public CertificateStatus CertificateStatus { get; set; }
  public string? CertificateTitle { get; set; }
  public DateTime? IssueDate { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public IEnumerable<Applications.FileInfo> Files { get; set; } = Array.Empty<Applications.FileInfo>();
  public IEnumerable<string> DeletedFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<string> NewFiles { get; set; } = Array.Empty<string>();
  public InternationalCertificationStatus Status { get; set; }
}

public enum CertificateStatus
{
  Valid,
  Expired
}

public record EmploymentReference
{
  public string? Id { get; set; }
  public string? LastName { get; set; }
  public string? FirstName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PhoneNumber { get; set; }
  public ICRAStatus? Status { get; set; }
}

public enum ICRAStatus
{
  Active,
  Draft,
  Eligible,
  Inactive,
  Ineligible,
  InReview,
  ReadyforReview,
  Submitted,
  ReadyforAssessment
}

public enum InternationalCertificationStatus
{
  ApplicationSubmitted,
  Approved,
  Draft,
  ICRAEligibilitySubmitted,
  Inactive,
  InProgress,
  Rejected,
  UnderReview,
  WaitingforResponse,
}

public record ICRAEligibilitySubmissionRequest(string Id);
public record SubmitICRAEligibilityResponse(ICRAEligibility Eligibility);
public record ResendIcraReferenceInviteResponse(string referenceId);

public record ICRAEligibilityStatus(string Id, DateTime? CreatedOn, DateTime? SignedDate, ICRAStatus Status)
{
  public IEnumerable<InternationalCertification> InternationalCertifications { get; set; } = Array.Empty<InternationalCertification>();
  public IEnumerable<EmploymentReferenceStatus> EmploymentReferencesStatus { get; set; } = Array.Empty<EmploymentReferenceStatus>();
  public bool AddAdditionalEmploymentExperienceReferences { get; set; }
}

public record EmploymentReferenceStatus(string Id, Applications.WorkExperienceRefStage? Status, string? FirstName, string? LastName, string? EmailAddress)
{
  public string? PhoneNumber { get; set; }
  public bool? WillProvideReference { get; set; }
}
