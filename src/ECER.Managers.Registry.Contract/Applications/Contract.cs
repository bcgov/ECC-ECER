using MediatR;

namespace ECER.Managers.Registry.Contract.Applications;

/// <summary>
/// Invokes draft application saving use case
/// </summary>
public record SaveDraftApplicationCommand(Application Application) : IRequest<string>;

/// <summary>
/// Invokes draft application saving use case
/// </summary>
public record CancelDraftApplicationCommand(string applicationId, string userId) : IRequest<string>;

/// <summary>
/// Invokes application submission use case
/// </summary>
/// <param name="applicationId"></param>
/// <param name="userId"></param>
public record SubmitApplicationCommand(string applicationId, string userId) : IRequest<ApplicationSubmissionResult>;

/// <summary>
/// Invokes application query use case
/// </summary>
public record ApplicationsQuery : IRequest<ApplicationsQueryResults>
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
}

/// <summary>
/// Container for <see cref="ApplicationsQuery"/> results
/// </summary>
/// <param name="Items">The </param>
public record ApplicationsQueryResults(IEnumerable<Application> Items);
/// <summary>
/// Invokes provinces use case
/// </summary>
public record ProvincesQuery : IRequest<ProvincesQueryResults>
{
  public string? ById { get; set; }
}
/// <summary>
/// Container for <see cref="ProvincesQuery"/> results
/// </summary>
/// <param name="Items">The </param>
public record ProvincesQueryResults(IEnumerable<Province> Items);

/// <summary>
/// Application submission result
/// </summary>
public record ApplicationSubmissionResult()
{
  public string? ApplicationId { get; set; }
  public SubmissionError? Error { get; set; }
  public IEnumerable<string>? ValidationErrors { get; set; }
  public bool IsSuccess { get { return ValidationErrors == null || !ValidationErrors.Any(); } }
}
public record Province(string ProvinceId, string ProvinceName);

public record Application(string? Id, string RegistrantId, ApplicationStatus Status)
{
  public DateTime? SubmittedOn { get; set; }
  public DateTime? CreatedOn { get; set; }
  public DateTime? SignedDate { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
  public PortalStage Stage { get; set; }
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
}

public record Transcript(string? Id, string? EducationalInstitutionName, string? ProgramName, string? StudentName, string? StudentNumber, DateTime StartDate, DateTime EndDate)
{
  public string? CampusLocation { get; set; }
  public string? LanguageofInstruction { get; set; }
}

public record WorkExperienceReference(string? FirstName, string? LastName, string? EmailAddress, int? Hours)
{
  public string? Id { get; set; }

  public string? PhoneNumber { get; set; }
}

public record CharacterReference(string? FirstName, string? LastName, string? PhoneNumber, string? EmailAddress)
{
  public string? Id { get; set; }
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

public enum CertificationType
{
  EceAssistant,
  OneYear,
  FiveYears,
  Ite,
  Sne
}

public enum SubmissionError
{
  DraftApplicationNotFound,
  DraftApplicationValidationFailed
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
  ReconsiderationDecision
}

public record ReferenceSubmissionRequest(string Token, ReferenceContactInformation ReferenceContactInformation, ReferenceEvaluation ReferenceEvaluation, bool ResponseAccuracyConfirmation) : IRequest<ReferenceSubmissionResult>;
public record ReferenceContactInformation(string LastName, string FirstName, string Email, string PhoneNumber, string CertificateNumber, string CertificateProvinceId, string CertificateProvinceOther);
public record ReferenceEvaluation(string Relationship, string LengthOfAcquaintance, bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment, bool ApplicantShouldNotBeECE, string ApplicantNotQualifiedReason);

public class ReferenceSubmissionResult
{
  public bool IsSuccess { get; set; }
  public string? ErrorMessage { get; set; }

  public static ReferenceSubmissionResult Success() => new ReferenceSubmissionResult { IsSuccess = true };

  public static ReferenceSubmissionResult Failure(string message) => new ReferenceSubmissionResult { IsSuccess = false, ErrorMessage = message };
}
