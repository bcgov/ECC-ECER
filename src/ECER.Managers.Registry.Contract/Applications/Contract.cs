﻿namespace ECER.Managers.Registry.Contract.Applications;

/// <summary>
/// Invokes draft application saving use case
/// </summary>
public record SaveDraftApplicationCommand(Application Application);

/// <summary>
/// Invokes application submission use case
/// </summary>
/// <param name="applicationId"></param>
public record SubmitApplicationCommand(string applicationId);

/// <summary>
/// Invokes application query use case
/// </summary>
public record ApplicationsQuery
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
}

/// <summary>
/// Container for <see cref="ApplicationsQuery"/> results
/// </summary>
/// <param name="Items">The </param>
public record ApplicationsQueryResults(IEnumerable<Application> Items);

public record Application(string? Id, string RegistrantId, ApplicationStatus Status)
{
  public DateTime? SubmittedOn { get; set; }
  public DateTime? CreatedOn { get; set; }
  public DateTime? SignedDate { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public PortalStage Stage { get; set; }
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
}
public record Transcript(string? Id, string? EducationalInstitutionName, string? ProgramName, string? StudentName, string? StudentNumber, DateTime StartDate, DateTime EndDate)
{
  public string? CampusLocation { get; set; }
  public string? LanguageofInstruction { get; set; }
}

public record CharacterReference
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; } 
  public string? PhoneNumber { get; set; } 
  public string? EmailAddress { get; set; }
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

