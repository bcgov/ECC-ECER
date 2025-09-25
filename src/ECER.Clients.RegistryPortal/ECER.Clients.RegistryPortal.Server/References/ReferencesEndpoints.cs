using AutoMapper;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.RegistryPortal.Server.References;

public class ReferencesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/References/Character", async Task<Results<Ok, BadRequest<ProblemDetails>>> (CharacterReferenceSubmissionRequest request, IMediator messageBus, HttpContext httpContext, IMapper mapper, CancellationToken ct) =>
    {
      if (request.Token == null) return TypedResults.BadRequest(new ProblemDetails() { Detail = "Token is required" });

      var captchaResult = await messageBus.Send(new Managers.Registry.Contract.Captcha.VerifyCaptchaCommand(request.CaptchaToken), ct);

      if (!captchaResult.Success)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Detail = "Invalid captcha token",
          Extensions = { ["errors"] = captchaResult.ErrorCodes }
        };
        return TypedResults.BadRequest(problemDetails);
      }

      var mappedCharacterReferenceRequest = mapper.Map<Managers.Registry.Contract.Applications.CharacterReferenceSubmissionRequest>(request);
      var result = await messageBus.Send(new Managers.Registry.Contract.Applications.SubmitReferenceCommand(request.Token)
      {
        CharacterReferenceSubmissionRequest = mappedCharacterReferenceRequest
      }, ct);
      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Detail = result.ErrorMessage });
      }
      return TypedResults.Ok();
    }).WithOpenApi("Handles character reference submission", string.Empty, "character_reference_post").WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/References/WorkExperience", async Task<Results<Ok, BadRequest<ProblemDetails>>> (WorkExperienceReferenceSubmissionRequest request, IMediator messageBus, HttpContext httpContext, IMapper mapper, CancellationToken ct) =>
    {
      if (request.Token == null) return TypedResults.BadRequest(new ProblemDetails { Detail = "Token is required" });

      var captchaResult = await messageBus.Send(new Managers.Registry.Contract.Captcha.VerifyCaptchaCommand(request.CaptchaToken), ct);

      if (!captchaResult.Success)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Detail = "Invalid captcha token",
          Extensions = { ["errors"] = captchaResult.ErrorCodes }
        };
        return TypedResults.BadRequest(problemDetails);
      }
      var mappedWorkExperienceRequest = mapper.Map<Managers.Registry.Contract.Applications.WorkExperienceReferenceSubmissionRequest>(request);
      var result = await messageBus.Send(new Managers.Registry.Contract.Applications.SubmitReferenceCommand(request.Token)
      {
        WorkExperienceReferenceSubmissionRequest = mappedWorkExperienceRequest
      }, ct);
      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(new ProblemDetails { Detail = result.ErrorMessage });
      }
      return TypedResults.Ok();
    }).WithOpenApi("Handles work experience reference submission", string.Empty, "workExperience_reference_post").WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/References/OptOut", async Task<Results<Ok, BadRequest<ProblemDetails>>> (OptOutReferenceRequest request, IMediator messageBus, HttpContext httpContext, IMapper mapper, CancellationToken ct) =>
    {
      if (request.Token == null) return TypedResults.BadRequest(new ProblemDetails { Detail = "Token is required" });

      var captchaResult = await messageBus.Send(new Managers.Registry.Contract.Captcha.VerifyCaptchaCommand(request.CaptchaToken), ct);

      if (!captchaResult.Success)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Detail = "Invalid captcha token",
          Extensions = { ["errors"] = captchaResult.ErrorCodes }
        };
        return TypedResults.BadRequest(problemDetails);
      }
      var result = await messageBus.Send(mapper.Map<Managers.Registry.Contract.Applications.OptOutReferenceRequest>(request), ct);
      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(new ProblemDetails { Detail = result.ErrorMessage });
      }
      return TypedResults.Ok();
    }).WithOpenApi("Handles reference optout", string.Empty, "reference_optout").WithParameterValidation();
  }
}

public record CharacterReferenceSubmissionRequest(string Token, bool WillProvideReference, ReferenceContactInformation ReferenceContactInformation, CharacterReferenceEvaluation ReferenceEvaluation, bool ConfirmProvidedInformationIsRight, [Required] string CaptchaToken);
public record ReferenceContactInformation([Required] string LastName, [Required] string Email, [Required] string PhoneNumber, string CertificateProvinceOther)
{
  public string? FirstName { get; set; }
  public string? CertificateProvinceId { get; set; }
  public string? CertificateNumber { get; set; }
  public DateTime? DateOfBirth { get; set; }
}
public record CharacterReferenceEvaluation([Required] ReferenceRelationship ReferenceRelationship, string ReferenceRelationshipOther, [Required] ReferenceKnownTime LengthOfAcquaintance, [Required] bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment);
public record OptOutReferenceRequest(string Token, [Required] UnabletoProvideReferenceReasons UnabletoProvideReferenceReasons, [Required] string CaptchaToken);

public enum UnabletoProvideReferenceReasons
{
  Iamunabletoatthistime,
  Idonothavetheinformationrequired,
  Idonotknowthisperson,
  Idonotmeettherequirementstoprovideareference,
  Other
}

public record WorkExperienceReferenceDetails()
{
  [Required]
  public int? Hours { get; set; }

  [Required]
  public WorkHoursType? WorkHoursType { get; set; }

  [Required]
  public string? ChildrenProgramName { get; set; }

  [RequiredWhenWorkExperienceType(WorkExperienceTypes.Is500Hours)]
  public ChildrenProgramType? ChildrenProgramType { get; set; }
  public string? ChildrenProgramTypeOther { get; set; }

  [RequiredWhenWorkExperienceType(WorkExperienceTypes.Is500Hours)]
  public IEnumerable<ChildcareAgeRanges>? ChildcareAgeRanges { get; set; }

  [RequiredWhenWorkExperienceType(WorkExperienceTypes.Is400Hours)]
  public string? Role { get; set; }
  public string? AgeofChildrenCaredFor { get; set; }

  [Required]
  public DateTime? StartDate { get; set; }

  [Required]
  public DateTime? EndDate { get; set; }

  [Required]
  public ReferenceRelationship? ReferenceRelationship { get; set; }
  public string? ReferenceRelationshipOther { get; set; }
  public string? AdditionalComments { get; set; }

  [Required]
  public WorkExperienceTypes? WorkExperienceType { get; set; }
}
public record WorkExperienceReferenceCompetenciesAssessment()
{
  [Required]
  public LikertScale? ChildDevelopment { get; set; }
  public string? ChildDevelopmentReason { get; set; }

  [Required]
  public LikertScale? ChildGuidance { get; set; }
  public string? ChildGuidanceReason { get; set; }

  [Required]
  public LikertScale? HealthSafetyAndNutrition { get; set; }
  public string? HealthSafetyAndNutritionReason { get; set; }

  [Required]
  public LikertScale? DevelopAnEceCurriculum { get; set; }
  public string? DevelopAnEceCurriculumReason { get; set; }

  [Required]
  public LikertScale? ImplementAnEceCurriculum { get; set; }
  public string? ImplementAnEceCurriculumReason { get; set; }

  [Required]
  public LikertScale? FosteringPositiveRelationChild { get; set; }
  public string? FosteringPositiveRelationChildReason { get; set; }

  [Required]
  public LikertScale? FosteringPositiveRelationFamily { get; set; }
  public string? FosteringPositiveRelationFamilyReason { get; set; }

  [Required]
  public LikertScale? FosteringPositiveRelationCoworker { get; set; }
  public string? FosteringPositiveRelationCoworkerReason { get; set; }
}
public record WorkExperienceReferenceSubmissionRequest([Required] string Token, bool WillProvideReference, ReferenceContactInformation ReferenceContactInformation, WorkExperienceReferenceDetails WorkExperienceReferenceDetails, [RequiredWhenWorkExperienceType(WorkExperienceTypes.Is500Hours)] WorkExperienceReferenceCompetenciesAssessment? WorkExperienceReferenceCompetenciesAssessment, bool ConfirmProvidedInformationIsRight, [Required] string CaptchaToken)
{
  [Required]
  public WorkExperienceTypes? WorkExperienceType { get; set; }
}

public enum WorkHoursType
{
  FullTime,
  PartTime,
}

public enum ChildrenProgramType
{
  Childminding,
  Familychildcare,
  Groupchildcare,
  InHomeMultiAgechildcare,
  MultiAgechildcare,
  Occasionalchildcare,
  Other,
  Preschool,
}

public enum ReferenceRelationship
{
  CoWorker,
  Other,
  ParentGuardianofChildinCare,
  Supervisor,
  Teacher,
}

public enum LikertScale
{
  Yes,
  No
}

#pragma warning disable CA1707

public enum ChildcareAgeRanges
{
  _036months,
  _35years,
  _68years,
}

public enum ReferenceKnownTime
{
  From1to2years,
  From2to5years,
  From6monthsto1year,
  Lessthan6months,
  Morethan5years,
}

// custom required annotations based on WorkExperienceTypes
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class RequiredWhenWorkExperienceTypeAttribute : ValidationAttribute
{
  private readonly WorkExperienceTypes _workExperienceType;
  public WorkExperienceTypes WorkExperienceType { get; }

  public RequiredWhenWorkExperienceTypeAttribute(WorkExperienceTypes workExperienceType)
  {
    _workExperienceType = workExperienceType;
  }

  protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
  {
    ArgumentNullException.ThrowIfNull(validationContext);
    var propertyName = validationContext.MemberName;

    var workExperienceReferenceType = validationContext.ObjectInstance.GetType()
    .GetProperty("WorkExperienceType")?
    .GetValue(validationContext.ObjectInstance);

    if (workExperienceReferenceType?.Equals(_workExperienceType) == true && value == null)
    {
      return new ValidationResult($"The {propertyName} field is required.");
    }

    return ValidationResult.Success!;
  }
}
