﻿namespace ECER.Resources.Documents.Certifications;

public interface ICertificationRepository
{
  Task<IEnumerable<Certification>> Query(UserCertificationQuery query);
}

public record UserCertificationQuery
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public string? ByApplicationId { get; set; }
}

public record Certification(string Id)
{
  public string? Number { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public DateTime? EffectiveDate { get; set; }
  public DateTime? Date { get; set; }
  public DateTime? PrintDate { get; set; }
  public bool? HasConditions { get; set; }
  public string? LevelName { get; set; }
  public CertificateStatusCode? StatusCode { get; set; }
  public YesNoNull? IneligibleReference { get; set; }
  public IEnumerable<CertificationLevel> Levels { get; set; } = Array.Empty<CertificationLevel>();
  public IEnumerable<CertificationFile> Files { get; set; } = Array.Empty<CertificationFile>();
}

public record CertificationLevel(string Id)
{
  public string? Type { get; set; }
}
public record CertificationFile(string Id)
{
  public string? Url { get; set; }
  public string? Extention { get; set; }
  public string? Size { get; set; }
  public string? Name { get; set; }
}

public enum CertificateStatusCode
{
  Active,
  Cancelled,
  Expired,
  Inactive,
  Renewed,
  Reprinted,
  Suspended
}

public enum YesNoNull
{
  No,
  Yes,
}
