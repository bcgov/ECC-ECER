﻿using AutoMapper;
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

      var recaptchaResult = await messageBus.Send(new Managers.Registry.Contract.Recaptcha.VerifyRecaptchaCommand(request.RecaptchaToken), ct);

      if (!recaptchaResult.Success)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Detail = "Invalid recaptcha token",
          Extensions = { ["errors"] = recaptchaResult.ErrorCodes }
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

      var recaptchaResult = await messageBus.Send(new Managers.Registry.Contract.Recaptcha.VerifyRecaptchaCommand(request.RecaptchaToken), ct);

      if (!recaptchaResult.Success)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Detail = "Invalid recaptcha token",
          Extensions = { ["errors"] = recaptchaResult.ErrorCodes }
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

      var recaptchaResult = await messageBus.Send(new Managers.Registry.Contract.Recaptcha.VerifyRecaptchaCommand(request.RecaptchaToken), ct);

      if (!recaptchaResult.Success)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Detail = "Invalid recaptcha token",
          Extensions = { ["errors"] = recaptchaResult.ErrorCodes }
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

public record CharacterReferenceSubmissionRequest(string Token, bool WillProvideReference, ReferenceContactInformation ReferenceContactInformation, CharacterReferenceEvaluation ReferenceEvaluation, bool ConfirmProvidedInformationIsRight, [Required] string RecaptchaToken);
public record ReferenceContactInformation([Required] string LastName, [Required] string FirstName, [Required] string Email, [Required] string PhoneNumber, string CertificateProvinceOther)
{
  public string? CertificateProvinceId { get; set; }
  public string? CertificateNumber { get; set; }
  public DateTime? DateOfBirth { get; set; }
}
public record CharacterReferenceEvaluation([Required] ReferenceRelationship ReferenceRelationship, string ReferenceRelationshipOther, [Required] ReferenceKnownTime LengthOfAcquaintance, [Required] bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment);
public record OptOutReferenceRequest(string Token, [Required] UnabletoProvideReferenceReasons UnabletoProvideReferenceReasons, [Required] string RecaptchaToken);

public enum UnabletoProvideReferenceReasons
{
  Iamunabletoatthistime,
  Idonothavetheinformationrequired,
  Idonotknowthisperson,
  Idonotmeettherequirementstoprovideareference,
  Other
}

public record WorkExperienceReferenceDetails([Required] int Hours, [Required] WorkHoursType WorkHoursType, [Required] string ChildrenProgramName, [Required] ChildrenProgramType ChildrenProgramType, string ChildrenProgramTypeOther, [Required] IEnumerable<ChildcareAgeRanges> ChildcareAgeRanges, [Required] DateTime StartDate, [Required] DateTime EndDate, [Required] ReferenceRelationship ReferenceRelationship, string ReferenceRelationshipOther);
public record WorkExperienceReferenceCompetenciesAssessment([Required] LikertScale ChildDevelopment, string ChildDevelopmentReason, [Required] LikertScale ChildGuidance, string ChildGuidanceReason, [Required] LikertScale HealthSafetyAndNutrition, string HealthSafetyAndNutritionReason, [Required] LikertScale DevelopAnEceCurriculum, string DevelopAnEceCurriculumReason, [Required] LikertScale ImplementAnEceCurriculum, string ImplementAnEceCurriculumReason, [Required] LikertScale FosteringPositiveRelationChild, string FosteringPositiveRelationChildReason, [Required] LikertScale FosteringPositiveRelationFamily, string FosteringPositiveRelationFamilyReason, [Required] LikertScale FosteringPositiveRelationCoworker, string FosteringPositiveRelationCoworkerReason);
public record WorkExperienceReferenceSubmissionRequest([Required] string Token, bool WillProvideReference, ReferenceContactInformation ReferenceContactInformation, WorkExperienceReferenceDetails WorkExperienceReferenceDetails, WorkExperienceReferenceCompetenciesAssessment WorkExperienceReferenceCompetenciesAssessment, bool ConfirmProvidedInformationIsRight, [Required] string RecaptchaToken);

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

public enum ChildcareAgeRanges
{
  From0to12Months,
  From12to24Months,
  From25to30Months,
  From31to36Months,
  Grade1,
  Preschool,
}

public enum ReferenceKnownTime
{
  From1to2years,
  From2to5years,
  From6monthsto1year,
  Lessthan6months,
  Morethan5years,
}
