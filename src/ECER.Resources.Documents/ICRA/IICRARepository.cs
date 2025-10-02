﻿namespace ECER.Resources.Documents.ICRA;

public interface IICRARepository
{
  Task<IEnumerable<ICRAEligibility>> Query(ICRAQuery query, CancellationToken cancellationToken);

  Task<string> Save(ICRAEligibility iCRAEligibility, CancellationToken cancellationToken);

  Task<string> Submit(string icraEligibilityId, CancellationToken cancellationToken);

  Task<string> SetIneligibleForUnitTests(string icraEligibilityId, CancellationToken cancellationToken);
}

public record ICRAQuery
{
  public string? ById { get; set; }
  public IEnumerable<ICRAStatus>? ByStatus { get; set; }
  public string? ByApplicantId { get; set; }
}

public record ICRAEligibility()
{
  public string? Id { get; set; }
  public string? PortalStage { get; set; }
  public string ApplicantId { get; set; } = string.Empty;
  public DateTime? SignedDate { get; set; }
  public DateTime? CreatedOn { get; set; }
  public ICRAStatus Status { get; set; }
  public IEnumerable<InternationalCertification> InternationalCertifications { get; set; } = Array.Empty<InternationalCertification>();
  public IEnumerable<EmploymentReference> EmploymentReferences { get; set; } = Array.Empty<EmploymentReference>();
}
public record InternationalCertification
{
  public string? Id { get; set; }
  public string? OtherFirstName { get; set; }
  public string? OtherMiddleName { get; set; }
  public string? OtherLastName { get; set; }
  public bool HasOtherName { get; set; }
  public string? CountryId { get; set; }
  public string? NameOfRegulatoryAuthority { get; set; }
  public string? EmailOfRegulatoryAuthority { get; set; }
  public string? PhoneOfRegulatoryAuthority { get; set; }
  public string? WebsiteOfRegulatoryAuthority { get; set; }
  public string? OnlineCertificateValidationToolOfRegulatoryAuthority { get; set; }
  public CertificateStatus CertificateStatus { get; set; }
  public string? CertificateTitle { get; set; }
  public DateTime? IssueDate { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public IEnumerable<Applications.FileInfo> Files { get; set; } = Array.Empty<Applications.FileInfo>();
  public IEnumerable<string> DeletedFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<string> NewFiles { get; set; } = Array.Empty<string>();
}

public record EmploymentReference
{
  public string? Id { get; set; }
  public string? LastName { get; set; }
  public string? FirstName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PhoneNumber { get; set; }
}

public enum CertificateStatus
{
  Valid,
  Expired
}

public enum ICRAStatus
{
  Active,
  Draft,
  Eligible,
  Inactive,
  Ineligible,
  InReview,
  ReadyforReview,
  Submitted,
  ReadyforAssessment
}
