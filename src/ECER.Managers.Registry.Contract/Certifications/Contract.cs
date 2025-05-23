﻿using ECER.Managers.Registry.Contract.Applications;
using MediatR;

namespace ECER.Managers.Registry.Contract.Certifications;

public record RequestCertificationPdfCommand(string certificationId, string userId) : IRequest<CertificationRequestPdfResult>;

public record UserCertificationQueryBase : IRequest<CertificationsQueryResults>
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public string? ByCertificateNumber { get; set; }
  public string? ByFirstName { get; set; }
  public string? ByLastName { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}

public record UserCertificationQuery : UserCertificationQueryBase;

public record UserCertificationQueryLookup : UserCertificationQueryBase;

public record CertificationsQueryResults(IEnumerable<Certification> Items);

public record Certification(string Id)
{
  public string? Name { get; set; }
  public string? Number { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public DateTime? EffectiveDate { get; set; }
  public DateTime? Date { get; set; }
  public DateTime? PrintDate { get; set; }
  public bool? HasConditions { get; set; }
  public string? LevelName { get; set; }
  public CertificatePDFGeneration? CertificatePDFGeneration { get; set; }
  public CertificateStatusCode? StatusCode { get; set; }
  public YesNoNull? IneligibleReference { get; set; }
  public IEnumerable<CertificationLevel> Levels { get; set; } = Array.Empty<CertificationLevel>();
  public IEnumerable<CertificationFile> Files { get; set; } = Array.Empty<CertificationFile>();
  public IEnumerable<CertificateCondition> CertificateConditions { get; set; } = Array.Empty<CertificateCondition>();
}

public record CertificateCondition
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public string? Details { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public int DisplayOrder { get; set; }
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
  public DateTime? CreatedOn { get; set; }
  public string? Tag1Name { get; set; }
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

public enum CertificatePDFGeneration
{
  No,
  Requested,
  Yes,
}

public enum YesNoNull
{
  No,
  Yes,
}

public record CertificationRequestPdfResult(string CertificationId)
{
}
