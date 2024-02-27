namespace ECER.Resources.Documents.Applications;

public interface IApplicationRepository
{
  Task<IEnumerable<Application>> Query(ApplicationQuery query);

  Task<string> SaveDraft(Application application);

  Task<string> Submit(string applicationId);
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
  public DateTime? SubmittedOn { get; set; }
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
}

public record Transcript(string? Id)
{
  public string EducationalInstitutionName { get; set; } = string.Empty;
  public string ProgramName { get; set; } = string.Empty;
  public string CampusLocation { get; set; } = string.Empty;
  public string StudentName { get; set; } = string.Empty;
  public string StudentNumber { get; set; } = string.Empty;
  public string LanguageofInstruction { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
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
