using AutoMapper;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.RegistryPortal.Server.References;

public class ReferencesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/References/Character", async Task<Results<Ok, BadRequest<string>>> (CharacterReferenceSubmissionRequest request, IMediator messageBus, HttpContext httpContext, IMapper mapper, CancellationToken ct) =>
    {
      if (request.Token == null) return TypedResults.BadRequest("Token is required");
      var result = await messageBus.Send(mapper.Map<Managers.Registry.Contract.Applications.CharacterReferenceSubmissionRequest>(request), ct);
      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(result.ErrorMessage);
      }
      return TypedResults.Ok();
    }).WithOpenApi("Handles character reference submission", string.Empty, "character_reference_post").WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/References/OptOut", async Task<Results<Ok, BadRequest<string>>> (OptOutReferenceRequest request, IMediator messageBus, HttpContext httpContext, IMapper mapper, CancellationToken ct) =>
    {
      if (request.Token == null) return TypedResults.BadRequest("Token is required");
      var result = await messageBus.Send(mapper.Map<Managers.Registry.Contract.Applications.OptOutReferenceRequest>(request), ct);
      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(result.ErrorMessage);
      }
      return TypedResults.Ok();
    }).WithOpenApi("Handles reference optout", string.Empty, "reference_optout").WithParameterValidation();
  }
}

public record CharacterReferenceSubmissionRequest(string Token, ReferenceContactInformation ReferenceContactInformation, CharacterReferenceEvaluation ReferenceEvaluation, bool ApplicantShouldNotBeECE, string ApplicantNotQualifiedReason, bool ConfirmProvidedInformationIsRight);
public record ReferenceContactInformation(string LastName, string FirstName, string Email, string PhoneNumber, string CertificateProvinceId, string CertificateProvinceOther)
{
  public string? CertificateNumber { get; set; }
  public string? DateOfBirth { get; set; }
}
public record CharacterReferenceEvaluation(string Relationship, string LengthOfAcquaintance, bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment);
public record OptOutReferenceRequest(string Token, UnabletoProvideReferenceReasons UnabletoProvideReferenceReasons);

public enum UnabletoProvideReferenceReasons
{
  Iamunabletoatthistime,
  Idonothavetheinformationrequired,
  Idonotknowthisperson,
  Idonotmeettherequirementstoprovideareference,
  Other
}

public record WorkExperienceReferenceDetails(int Hours, WorkHoursType WorkHoursType, string ChildrenProgramName, ChildrenProgramType ChildrenProgramType, string ChildrenProgramTypeOther, string AgeOfChildrenCaredFor, DateTime StartDate, DateTime EndDate, ReferenceRelationship ReferenceRelationship, string ReferenceRelationshipOther);
public record WorkExperienceReferenceCompetenciesAssessment(LikertScale ChildDevelopment, LikertScale ChildGuidance, LikertScale HealthSafetyAndNutrition, LikertScale DevelopAnEceCurriculum, LikertScale ImplementAnEceCurriculum, LikertScale FosteringPositiveRelationChild, LikertScale FosteringPositiveRelationFamily, LikertScale FosteringPositiveRelationCoworker);
public record WorkExperienceReferenceSubmissionRequest(string Token, ReferenceContactInformation ReferenceContactInformation, WorkExperienceReferenceDetails WorkExperienceReferenceDetails, WorkExperienceReferenceCompetenciesAssessment WorkExperienceReferenceCompetenciesAssessment, bool ApplicantShouldNotBeECE, string ApplicantNotQualifiedReason, bool ConfirmProvidedInformationIsRight);

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
  Supervisor,
}

public enum LikertScale
{
  SomewhatAgree,
  SomewhatDisagree,
  StronglyAgree,
  StronglyDisagree,
}
