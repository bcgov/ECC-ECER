namespace ECER.Resources.Applications;

public interface IApplicationRepository
{
    Task<IEnumerable<Application>> Query(ApplicationQuery query);

    Task<string> SaveDraft(Application application);

    Task<string> Submit(string applicationId);
}

public abstract record ApplicationQuery
{
    public string? ById { get; set; }
}

public record CertificationApplicationQuery : ApplicationQuery
{
    public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
    public string? ByApplicantId { get; set; }
}

public abstract record Application(string? Id)
{
    public ApplicationStatus Status { get; set; }
    public DateTime CreateddOn { get; set; }
    public DateTime? SubmittedOn { get; set; }
}

public record CertificationApplication(string? Id, string ApplicantId, IEnumerable<CertificationType> CertificationTypes) : Application(Id)
{
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
    ReviewforCompletness,
    ReadyforAssessment,
    BeingAssessed,
    Reconsideration,
    Appeal,
    Cancelled,
}