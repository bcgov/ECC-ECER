namespace ECER.Resources.Documents.Certifications;

public interface ICertificationRepository
{
  Task<IEnumerable<Certification>> Query(UserCertificationQuery query);
}

public record UserCertificationQuery
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
}

public record Certification(string Id)
{
  public string? Number { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public DateTime? EffectiveDate { get; set; }
  public DateTime? Date { get; set; }
  public DateTime? PrintDate { get; set; }
  public bool? HasConditions { get; set; }
  public string? Level { get; set; }
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
