using ECER.Resources.Documents.Shared;

namespace ECER.Resources.Documents.Programs;

public interface IProgramRepository
{
  Task<ProgramResult> Query(ProgramQuery query, CancellationToken cancellationToken);

  Task<string> Save(Program program, CancellationToken cancellationToken);
  Task<string> UpdateProgram(Program program, CancellationToken cancellationToken);
  Task<string> SubmitProgramProfile(string id, string userId, CancellationToken cancellationToken);
  Task<string> ChangeProgram(Program program, CancellationToken cancellationToken);
}

public record ProgramQuery
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ProgramStatus>? ByStatus { get; set; }
  public string? ByFromProgramProfileId { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}

public record Program(string? Id, string PostSecondaryInstituteId)
{
  public string? PortalStage { get; set; }
  public ProgramStatus Status { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? Name { get; set; }
  public string? ProgramName { get; set; }
  public string? PostSecondaryInstituteName { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public float? NewBasicTotalHours { get; set; }
  public float? NewSneTotalHours { get; set; }
  public float? NewIteTotalHours { get; set; }
  public string? DeclarationDate { get; set; }
  public string? DeclarationUserName { get; set; }
  public bool ChangesMade { get; set; }
  public ProgramProfileType ProgramProfileType { get; set; }
  public IEnumerable<string>? ProgramTypes { get; set; }
  public IEnumerable<Course>? Courses { get; set; }
  public IEnumerable<string>? OfferedProgramTypes { get; set; }
  public string? FromProgramProfileId { get; set; }
  public bool? ReadyForReview { get; set; }
}

public enum ProgramStatus
{
  Draft,
  UnderReview,
  Approved,
  Denied,
  Inactive,
  ChangeRequestInProgress,
  Withdrawn
}

public enum ProgramProfileType
{
  ChangeRequest,
  AnnualReview
}

public record ProgramResult
{
  public IEnumerable<Program>? Programs { get; set; }
  public int TotalProgramsCount { get; set; }
}
