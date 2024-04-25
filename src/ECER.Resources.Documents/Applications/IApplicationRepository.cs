using ECER.Resources.Documents.PortalInvitations;

namespace ECER.Resources.Documents.Applications;

public interface IApplicationRepository
{
  Task<IEnumerable<Application>> Query(ApplicationQuery query, CancellationToken cancellationToken);

  Task<string> SaveDraft(Application application, CancellationToken cancellationToken);

  Task<string> Submit(string applicationId, CancellationToken cancellationToken);

  Task<string> Cancel(string applicationId, CancellationToken cancellationToken);

  Task<string> SubmitCharacterReference(CharacterReferenceSubmissionRequest request, CancellationToken cancellationToken);

  Task<string> SubmitWorkexperienceReference(CharacterReferenceSubmissionRequest request, CancellationToken cancellationToken);

  Task<string> OptOutCharacterReference(OptOutReferenceRequest request, CancellationToken cancellationToken);

  Task<string> OptOutWorkExperienceReference(OptOutReferenceRequest request, CancellationToken cancellationToken);
}

public record ApplicationQuery
{
  public string? ById { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
  public string? ByApplicantId { get; set; }
}

public record Application(string? Id, string ApplicantId, IEnumerable<CertificationType> CertificationTypes)
{
  public ApplicationStatus Status { get; set; }
  public DateTime CreatedOn { get; set; }
  public DateTime? SignedDate { get; set; }
  public DateTime? SubmittedOn { get; set; }
  public PortalStage Stage { get; set; }
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
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

public record CharacterReference(string? FirstName, string? LastName, string? PhoneNumber, string? EmailAddress)
{
  public string? Id { get; set; }
}

public record OptOutReferenceRequest(UnabletoProvideReferenceReasons UnabletoProvideReferenceReasons)
{
  public PortalInvitation? PortalInvitation { get; set; }
}

public enum UnabletoProvideReferenceReasons
{
  Iamunabletoatthistime,
  Idonothavetheinformationrequired,
  Idonotknowthisperson,
  Idonotmeettherequirementstoprovideareference,
  Other
}

public record CharacterReferenceSubmissionRequest(ReferenceContactInformation ReferenceContactInformation, CharacterReferenceEvaluation ReferenceEvaluation, bool ApplicantShouldNotBeECE, string ApplicantNotQualifiedReason, bool ConfirmProvidedInformationIsRight)
{
  public PortalInvitation? PortalInvitation { get; set; }
}
public record ReferenceContactInformation(string LastName, string FirstName, string Email, string PhoneNumber, string CertificateProvinceId, string CertificateProvinceOther)
{
  public string? CertificateNumber { get; set; }
  public string? DateOfBirth { get; set; }
}
public record CharacterReferenceEvaluation(string Relationship, string LengthOfAcquaintance, bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment);
