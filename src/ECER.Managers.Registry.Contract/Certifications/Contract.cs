using MediatR;

namespace ECER.Managers.Registry.Contract.Certifications;

public record UserCertificationQuery : IRequest<CertificationsQueryResults>
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
}
public record CertificationsQueryResults(IEnumerable<Certification> Items);

public record Certification(string Id)
{
  public string? Number { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public DateTime? EffectiveDate { get; set; }
  public DateTime? Date { get; set; }
  public bool? HasConditions { get; set; }
  public bool? Level { get; set; }
  public CertificateStatusCode? StatusCode { get; set; }
  public YesNoNull? IneligibleReference { get; set; }
}

public enum CertificateStatusCode
{
  Active,
  Cancelled,
  Expired,
  Inactive,
  Reprinted,
  Suspended
}

public enum YesNoNull
{
  No,
  Yes,
}
