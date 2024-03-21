namespace ECER.Resources.Documents.Applications;

public interface IApplicationRepository
{
  Task<IEnumerable<Application>> Query(ApplicationQuery query);

  Task<string> SaveDraft(Application application);

  Task<string> Submit(string applicationId);

  Task<string> Delete(string applicationId, CancellationToken ct);
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
  ReconsiderationDecision
}
public record CharacterReference(string? FirstName, string? LastName, string? PhoneNumber, string? EmailAddress)
{
  public string? Id { get; set; }
}
