using ECER.Utilities.Security;

namespace ECER.Managers.Registry.Contract.Applications;

/// <summary>
/// Saves an application in draft state
/// </summary>
public record SaveDraftCertificationApplicationCommand(CertificationApplication Application);

public record SubmitCertificationApplicationCommand(string applicationId);

public record CertificationApplicationsQuery
{
  public string? ById { get; set; }
  public UserIdentity? ByIdentity { get; set; }
}

public record ApplicationsQueryResults(IEnumerable<CertificationApplication> Items);

public record CertificationApplication
{
  public string? Id { get; set; }
  public string RegistrantId { get; set; } = null!;
  public DateTime SubmittedOn { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
}

public enum CertificationType
{
  OneYear,
  FiveYears
}
